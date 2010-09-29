#define DEBUG
using System;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class InvadeAdviser : BaseAdviser
	{
		public InvadeAdviser(PlanetWars context)
			: base(context)
		{
		}

		public override Moves Run()
		{
			Moves moves = new Moves();

			Planets neutralPlanets = Context.NeutralPlanets();
			if (neutralPlanets.Count == 0) return moves;

			if (neutralPlanets.Count > 1)
				neutralPlanets.Sort(new Comparer(Context).CompareImportanceOfNeutralPlanetsGT);

			bool noNearPlanets = true;
			foreach (Planet planet in neutralPlanets)
			{
				if (usedPlanets.IndexOf(planet) != -1) continue;
				usedPlanets.Add(planet);

				Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);
				if (nearestPlanets.Count == 0) continue;
				noNearPlanets = false;

				if (nearestPlanets.Count > 1)
					nearestPlanets.Sort(new Comparer(Context).CompareNumberOfShipsGT);

#if DEBUG
				Logger.Log("      Trying to invade planet " + planet.PlanetID() + "...");
#endif

				int sendedShipsNum = Context.GetFleetsShipNum(Context.MyFleetsGoingToPlanet(planet));
				int maxNeedToSend = 0;
				foreach (Planet nearPlanet in nearestPlanets)
				{
					int distance = Context.Distance(planet, nearPlanet);
					Planet futurePlanet = Context.PlanetFutureStatus(planet, distance + Config.ExtraTurns);
					int needToSend = Config.MinShipsOnPlanetsAfterInvade;
					if (futurePlanet.Owner() != 1)
					{
						needToSend += futurePlanet.NumShips();
					}
					else
					{
						needToSend -= futurePlanet.NumShips();
					}

					if (maxNeedToSend < needToSend) maxNeedToSend = needToSend;

					int canSend = Math.Min(maxNeedToSend - sendedShipsNum, Context.CanSend(nearPlanet));
					if (canSend <= 0) continue;
					moves.Add(new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend));
					sendedShipsNum += canSend;
				}
				if (sendedShipsNum < maxNeedToSend)
				{
					moves.Clear();
#if DEBUG
					//Logger.Log("      failed");
#endif
				} else
				{
#if DEBUG
					//Logger.Log(moves.Count == 0 ? "      no need to send" : "      accepted!");
#endif
					break;
				}
			}
			if (noNearPlanets)
			{
				Config.IncInvadeDistance();
			}
			else
			{
				Config.ResetInvadeDistance();
			}
#if DEBUG
			//Logger.Log("      InvadeDistance " + Config.InvokeDistanceForInvade);
#endif

			return moves;
		}

		public override string GetAdviserName()
		{
			return "Invade";
		}
	}
}
