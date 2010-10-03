using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class StealAdviser : BaseAdviser
	{
		public StealAdviser(PlanetWars context)
			: base(context)
		{
		}

		private Planet SelectPlanetForAdvise()
		{
			Planets planets = Context.NeutralPlanetsUnderAttack();

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = planets.IndexOf(usedPlanet);
					if (index != -1) planets.RemoveAt(index);
				}
			}

			if (planets.Count == 0)
			{
				IsWorkFinished = true;
				return null;
			}

			planets.Sort(new Comparer(Context).CompareGrowsRateGT);

			usedPlanets.Add(planets[0]);
			return planets[0];
		}

		public override Moves Run()
		{
			Moves moves = new Moves();

			Planet stealPlanet = SelectPlanetForAdvise();
			if (stealPlanet == null) return moves;

			if (Context.MyPlanets().Count == 0)
			{
				IsWorkFinished = true;
				return moves;
			}

			Fleets enemyFleets = Context.EnemyFleetsGoingToPlanet(stealPlanet);
			if (enemyFleets.Count == 0) return moves;
			enemyFleets.Sort(new Comparer(Context).CompareTurnsRemainingLT);
			Fleets firstFleets = Context.GetThisTurnFleets(enemyFleets[0].TurnsRemaining(), enemyFleets);
			Fleets secondFleets = Context.GetThisTurnFleets(enemyFleets[0].TurnsRemaining() + 1, enemyFleets);

			Planets myPlanets = Context.MyPlanetsWithinProximityToPlanet(stealPlanet, enemyFleets[0].TurnsRemaining() + 1);
			if (myPlanets.Count == 0) return moves;

			Planet futurePlanet = Context.PlanetFutureStatus(stealPlanet, enemyFleets[0].TurnsRemaining() + 1);
			if (futurePlanet.Owner() != 2) return moves;
			foreach (Planet myPlanet in myPlanets)
			{
				if (Context.Distance(stealPlanet, myPlanet) == enemyFleets[0].TurnsRemaining() + 1)
				{
					int canSend = Context.CanSend(myPlanet);
					if (canSend > futurePlanet.NumShips())
						moves.Add(new Move(myPlanet.PlanetID(), stealPlanet.PlanetID(), canSend));
				}
			}
			
			//TODO Check if we can defend our steal
			//Context.GetFleetsShipNum(enemyFleets);

			return moves;
		}

		public override string GetAdviserName()
		{
			return "Steal";
		}
	}
}
