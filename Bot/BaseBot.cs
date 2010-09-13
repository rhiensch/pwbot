using System.Collections.Generic;
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

		private static int CompareNumberOfShipsLT(Planet planet1, Planet planet2)
		{
			return (planet1.NumShips() - planet2.NumShips());
		}

		private static int CompareNumberOfShipsGT(Planet planet1, Planet planet2)
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
			Planets strongestPlanets = new Planets(number);
			if (number == 1)
			{
				Planet strongestPlanet = planets[0];

				foreach (Planet planet in planets)
				{
					if (planet.NumShips() > strongestPlanet.NumShips())
					{
						strongestPlanet = planet;
					}
				}
				strongestPlanets.Add(strongestPlanet);
			}
			else if (number != 0)
			{
				Planets sortedPlanets = planets;
				sortedPlanets.Sort(CompareNumberOfShipsGT);

				if (number > sortedPlanets.Count)
				{
					number = planets.Count;
				}

				strongestPlanets = sortedPlanets.GetRange(0, number);
			}

			return strongestPlanets;
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

		private static int CompareSecondOfPair(Pair<int, int> pair1, Pair<int, int> pair2)
		{
			return pair2.Second - pair1.Second;
		}

		public Planet PlanetFutureStatus(Planet planet, int numberOfTurns)
		{
			Planet planetInFuture = new Planet(planet);

			// All fleets heading to this planet
			Fleets thisPlanetFleets = FleetsGoingToPlanet(Context.Fleets(), planet);

			for (uint turn = 1; turn <= numberOfTurns; turn++)
			{
				PlanetGrowth(planetInFuture);

				// First is ownerID, second is number of ships
				List<Pair<int, int>> ships = new List<Pair<int, int>>();

				// Get all fleets whic will arrive at the planet in this turn
				Fleets thisTurnFleets = GetThisTurnFleets(turn, thisPlanetFleets);

				CalcFleetsOnPlanet(planetInFuture, thisTurnFleets, ships);
			}
			return planetInFuture;
		}

		private static Fleets GetThisTurnFleets(uint turn, Fleets thisPlanetFleets)
		{
			Fleets thisTurnFleets = new Fleets();
			foreach (Fleet fleet in thisPlanetFleets)
			{
				if (fleet.TurnsRemaining() == turn)
				{
					thisTurnFleets.Add(fleet);
				}
			}
			return thisTurnFleets;
		}

		private void CalcFleetsOnPlanet(Planet planetInFuture, Fleets thisTurnFleets, List<Pair<int, int>> ships)
		{
			if (thisTurnFleets.Count > 0)
			{
				const int owners = 2;

				for (int id = 1; id <= owners; ++id)
				{
					Fleets ownerFleets = FleetsWithGivenOwner(thisTurnFleets, id);
					Pair<int, int> ownerShips = new Pair<int, int>(id, 0);

					// Add up fleets with the same owner
					foreach (Fleet ownerFleet in ownerFleets)
					{
						ownerShips.Second += ownerFleet.NumShips();
					}

					// Add the ships from the planet to the corresponding fleet
					if (planetInFuture.Owner() == id)
					{
						ownerShips.Second += planetInFuture.NumShips();
					}

					ships.Add(ownerShips);
				}

				// If the planet was neutral, it has it's own fleet
				if (planetInFuture.Owner() == 0)
				{
					ships.Add(new Pair<int, int>(0, planetInFuture.NumShips()));
				}

				BattleForPlanet(planetInFuture, ships);
			}
		}

		private static void BattleForPlanet(Planet planetInFuture, List<Pair<int, int>> ships)
		{
			// Were there any fleets other than the one on the planet?
			if (ships.Count > 1)
			{
				// Sorts the fleets in descending order by the number of ships in the fleet
				ships.Sort(CompareSecondOfPair);

				Pair<int, int> winner = ships[0];
				Pair<int, int> secondToWinner = ships[1];

				if (winner.Second == secondToWinner.Second)
				{
					planetInFuture.Owner(0);
					planetInFuture.NumShips(0);
				}
				else
				{
					planetInFuture.Owner(winner.First);
					planetInFuture.NumShips(winner.Second - secondToWinner.Second);
				}
			}
		}

		private static void PlanetGrowth(Planet planetInFuture)
		{
			if (planetInFuture.Owner() != 0)
			{
				planetInFuture.NumShips(planetInFuture.NumShips() + planetInFuture.GrowthRate());
			}
		}

		public Planets MyEndangeredPlanets(int numberOfTurns, int treshold)
		{
			Planets planets = Context.MyPlanets();
			Planets endangeredPlanets = new Planets();

			foreach (Planet planet in planets)
			{
				Planet planeInFuture = PlanetFutureStatus(planet, numberOfTurns);
				if ((planeInFuture.Owner() != 1) || (planeInFuture.NumShips() <= treshold))
				{
					endangeredPlanets.Add(planet);
				}
			}

			return endangeredPlanets;
		}
	}
}
