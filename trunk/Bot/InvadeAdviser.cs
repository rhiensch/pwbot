#undef DEBUG
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

		private Planet SelectPlanetForAdvise()
		{
			Planets neutralPlanets = Context.NeutralPlanets();

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = neutralPlanets.IndexOf(usedPlanet);
					if (index != -1) neutralPlanets.RemoveAt(index);
				}
			}

			if (neutralPlanets.Count == 0)
			{
				IsWorkFinished = true;
				return null;
			}

			Planets neutralPlanetsCloseEnough = new Planets(neutralPlanets);
			foreach (Planet neutralPlanet in neutralPlanets)
			{
				Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(neutralPlanet, Config.InvokeDistanceForInvade);
				if (nearestPlanets.Count == 0)
				{
					usedPlanets.Add(neutralPlanet);
					neutralPlanetsCloseEnough.Remove(neutralPlanet);
				}
			}

			if (neutralPlanetsCloseEnough.Count == 0)
			{
				Config.IncInvadeDistance();
				IsWorkFinished = true;
				return null;
			}
			Config.ResetInvadeDistance();

			if (neutralPlanetsCloseEnough.Count == 1)
			{
				usedPlanets.Add(neutralPlanetsCloseEnough[0]);
				return neutralPlanets[0];
			}

			neutralPlanetsCloseEnough.Sort(new Comparer(Context).CompareImportanceOfNeutralPlanetsGT);
			usedPlanets.Add(neutralPlanetsCloseEnough[0]);
			
			return neutralPlanetsCloseEnough[0];
		}

		public override Moves Run()
		{
			Moves moves = new Moves();

			Planet planet = SelectPlanetForAdvise();
			if (planet == null) return moves;

			Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);
			if (nearestPlanets.Count == 0) return moves;

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
			}
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Invade";
		}
	}
}
