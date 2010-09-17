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

			neutralPlanets.Sort(new Comparer(Context).CompareImportanceOfPlanetsGT);

			foreach (Planet planet in neutralPlanets)
			{
				//TODO hardcode
				Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(planet, 20);
				if (nearestPlanets.Count == 0) continue;

				nearestPlanets.Sort(new Comparer(Context).CompareNumberOfShipsGT);

				int needToSend = planet.NumShips() + 10;
				int sendedShipsNum = Context.GetFleetsShipNum(Context.MyFleetsGoingToPlanet(planet));
				foreach (Planet nearPlanet in nearestPlanets)
				{
					int canSend = Math.Min(needToSend - sendedShipsNum, nearPlanet.NumShips() - 10);
					if (canSend <= 0) continue;
					moves.Add(new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend));
					sendedShipsNum += canSend;
				}
				if (sendedShipsNum < planet.NumShips()) moves.Clear();
			}

			return moves;
		}
	}
}
