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
				neutralPlanets.Sort(new Comparer(Context).CompareImportanceOfPlanetsGT);

			foreach (Planet planet in neutralPlanets)
			{
				Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);
				if (nearestPlanets.Count == 0) continue;

				if (nearestPlanets.Count > 1)
					nearestPlanets.Sort(new Comparer(Context).CompareNumberOfShipsGT);

				
				int sendedShipsNum = Context.GetFleetsShipNum(Context.MyFleetsGoingToPlanet(planet));
				int maxNeedToSend = 0;
				foreach (Planet nearPlanet in nearestPlanets)
				{
					int distance = Context.Distance(planet, nearPlanet);
					Planet futurePlanet = Context.PlanetFutureStatus(planet, distance + Config.ExtraTurns);
					int needToSend = Config.MinShipsOnMyPlanetsAfterInvade;
					if (futurePlanet.Owner() != 1)
					{
						needToSend += futurePlanet.NumShips();
					}
					else
					{
						needToSend -= futurePlanet.NumShips();
					}

					//TODO maybe check for endangared?
					if (Context.EnemyFleetsGoingToPlanet(nearPlanet).Count > 0) continue;

					int canSend = Math.Min(needToSend - sendedShipsNum, nearPlanet.NumShips() - Config.MinShipsOnMyPlanetsAfterInvade);
					if (canSend <= 0) continue;
					moves.Add(new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend));
					sendedShipsNum += canSend;

					if (maxNeedToSend < needToSend) maxNeedToSend = needToSend;
				}
				if (sendedShipsNum < maxNeedToSend) moves.Clear(); else break;
			}

			return moves;
		}

		public override string GetAdviserName()
		{
			return "Invade";
		}
	}
}
