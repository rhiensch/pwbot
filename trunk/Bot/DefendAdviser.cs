using System;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class DefendAdviser : BaseAdviser
	{
		public DefendAdviser(PlanetWars context)
			: base(context)
		{
		}

		public override Moves Run()
		{
			Moves moves = new Moves();

			//TODO replace hardcode parameters
			Planets myEndangeredPlanets = Context.MyEndangeredPlanets(5, 10);

			if (myEndangeredPlanets.Count == 0) return moves;

			Planets planets = Context.MostImportantPlanets(myEndangeredPlanets, 1);

			if (planets.Count == 0) return moves;

			Planet planet = planets[0];

			Fleets enemyFleets = Context.EnemyFleetsGoingToPlanet(planet);
			if (enemyFleets.Count == 0) 
				return moves; //if we return here, then something is wrong

			int enemyShipsNum = Context.GetFleetsShipNum(enemyFleets);
			int turnsBeforeAttack = Context.GetClosestFleetDistance(enemyFleets);

			Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(planet, turnsBeforeAttack + 3);
			nearestPlanets.Sort(Context.CompareNumberOfShipsGT);

			//Planet planetNow = Context.GetPlanet(planet.PlanetID());
			int sendedShipsNum = planet.NumShips() + planet.GrowthRate() * turnsBeforeAttack;
			foreach (Planet nearPlanet in nearestPlanets)
			{
				int canSend = Math.Min(enemyShipsNum - sendedShipsNum, nearPlanet.NumShips() - 10);
				if (canSend <= 0) continue;
				moves.Add(new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend));
				sendedShipsNum += canSend;
			}

			return moves;
		}
	}
}
