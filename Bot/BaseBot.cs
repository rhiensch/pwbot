using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class BaseBot
	{
		public PlanetWars Context { get; private set; }
		public BaseBot(PlanetWars planetWars)
		{
			Context = planetWars;
		}

		private int CompareNumberOfShipsLT(Planet planet1, Planet planet2)
		{
			return (planet1.NumShips() - planet2.NumShips());
		}

		private int CompareNumberOfShipsGT(Planet planet1, Planet planet2)
		{
			return (planet2.NumShips() - planet1.NumShips());
		}

		//Any planets
		public Planets WeakestPlanets(Planets planets, int number)
		{
			Planets weakestPlanets = new Planets(number);
			if (number == 1)
			{
				Planet weakestPlanet = planets[0];

				foreach (Planet planet in planets)
				{
					if (planet.NumShips() < weakestPlanet.NumShips())
					{
						weakestPlanet = planet;
					}
				}
				weakestPlanets.Add(weakestPlanet);
			}
			else if (number != 0)
			{
				Planets sortedPlanets = planets;
				sortedPlanets.Sort(CompareNumberOfShipsLT);

				if (number > sortedPlanets.Count)
				{
					number = planets.Count;
				}

				weakestPlanets = sortedPlanets.GetRange(0, number);
			}
			return weakestPlanets;
		}

		public Planets StrongestPlanets(Planets planets, int number)
		{
			Planets weakestPlanets = new Planets(number);
			if (number == 1)
			{
				Planet weakestPlanet = planets[0];

				foreach (Planet planet in planets)
				{
					if (planet.NumShips() > weakestPlanet.NumShips())
					{
						weakestPlanet = planet;
					}
				}
				weakestPlanets.Add(weakestPlanet);
			}
			else if (number != 0)
			{
				Planets sortedPlanets = planets;
				sortedPlanets.Sort(CompareNumberOfShipsLT);
				sortedPlanets.Reverse();

				if (number > sortedPlanets.Count)
				{
					number = planets.Count;
				}

				weakestPlanets = sortedPlanets.GetRange(0, number);
			}

			return weakestPlanets;
		}

		//My Planets
		public Planets MyWeakestPlanets(int number)
		{
			return WeakestPlanets(Context.MyPlanets(), number);
		}

		public Planets MyStrongestPlanets(int number)
		{
			return StrongestPlanets(Context.MyPlanets(), number);
		}

		//Neutral Planets
		public Planets NeutralWeakestPlanets(int number)
		{
			return WeakestPlanets(Context.NeutralPlanets(), number);
		}

		public Planets NeutralStrongestPlanets(int number)
		{
			return StrongestPlanets(Context.NeutralPlanets(), number);
		}

		//Opponents Planets
		public Planets EnemyWeakestPlanets(int number)
		{
			return WeakestPlanets(Context.EnemyPlanets(), number);
		}

		public Planets EnemyStrongestPlanets(int number)
		{
			return StrongestPlanets(Context.EnemyPlanets(), number);
		}

		public Planets PlanetsWithGivenOwner(Planets planets, int ownerID)
		{
			if (ownerID == -1)
			{
				return planets;
			}

			Planets selectedPlanets = new Planets();

			foreach (Planet planet in planets)
			{
				if (planet.Owner() == ownerID)
				{
					selectedPlanets.Add(planet);
				}
			}
			return selectedPlanets;
		}

		public Fleets FleetsWithGivenOwner(Fleets fleets, int ownerID)
		{
			if (ownerID == -1)
			{
				return fleets;
			}
			Fleets selectedFleets = new Fleets();

			foreach (Fleet fleet in fleets)
			{
				if (fleet.Owner() == ownerID)
				{
					selectedFleets.Add(fleet);
				}
			}
			return selectedFleets;
		}

		public Fleets FleetsGoingToPlanet(Fleets fleets, Planet planet)
		{
			Fleets attackingFleets = new Fleets();
			foreach (Fleet fleet in fleets)
			{
				if (fleet.DestinationPlanet() == planet.PlanetID())
				{
					attackingFleets.Add(fleet);
				}
			}
			return attackingFleets;
		}

		public Fleets MyFleetsGoingToPlanet(Planet planet)
		{
			return FleetsGoingToPlanet(Context.MyFleets(), planet);
		}

		public Fleets EnemyFleetsGoingToPlanet(Planet planet)
		{
			return FleetsGoingToPlanet(Context.EnemyFleets(), planet);
		}

		/*public Planet PlanetFutureStatus(Planet planet, int numberOfTurns)
		{
			
		}*/

		private Planets PlanetsUnderAttack(int ownerID)
		{
			Fleets enemyFleets = Context.EnemyFleets();
			Planets attackedPlanets = new Planets();

			foreach (Fleet enemyFleet in enemyFleets)
			{
				Planet planet = Context.GetPlanet(enemyFleet.DestinationPlanet());
				if (planet.Owner() == ownerID)
				{
					attackedPlanets.Add(planet);
				}
			}
			return attackedPlanets;
		}

		public Planets MyPlanetsUnderAttack()
		{
			return PlanetsUnderAttack(1);
		}

		public Planets NeutralPlanetsUnderAttack()
		{
			return PlanetsUnderAttack(0);
		}

		public Planets PlanetsWithinProximityToPlanet(Planets planets, Planet thisPlanet, int proximityTreshold)
		{
			Planets nearbyPlanets = new Planets();

			foreach (Planet planet in planets)
			{
				if (planet.PlanetID() == thisPlanet.PlanetID())
				{
					continue;
				}
				int distance = Context.Distance(planet.PlanetID(), thisPlanet.PlanetID());
				if (distance <= proximityTreshold)
				{
					nearbyPlanets.Add(planet);
				}
			}
			return nearbyPlanets;
		}

		public Planets MyPlanetsWithinProximityToPlanet(Planet thisPlanet, int proximityTreshold)
		{
			return PlanetsWithinProximityToPlanet(Context.MyPlanets(), thisPlanet, proximityTreshold);
		}

		public Planets NeutralPlanetsWithinProximityToPlanet(Planet thisPlanet, int proximityTreshold)
		{
			return PlanetsWithinProximityToPlanet(Context.NeutralPlanets(), thisPlanet, proximityTreshold);
		}

		public Planets EnemyPlanetsWithinProximityToPlanet(Planet thisPlanet, int proximityTreshold)
		{
			return PlanetsWithinProximityToPlanet(Context.EnemyPlanets(), thisPlanet, proximityTreshold);
		}
	}
}
