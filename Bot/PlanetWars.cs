// Contestants do not need to worry about anything in this file. This is just
// helper code that does the boring stuff for you, so you can focus on the
// interesting stuff. That being said, you're welcome to change anything in
// this file if you know what you're doing.

using System;
using System.Collections.Generic;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class PlanetWars
	{
		private const int GROWS_RATE_KOEF = 1;
		private const int DISTANCE_KOEF = -1;

		// Constructs a PlanetWars object instance, given a string containing a
		// description of a game state.
		public PlanetWars(string gameStatestring)
		{
			planets = new List<Planet>();
			fleets = new Fleets();
			ParseGameState(gameStatestring);
		}

		// Returns the number of planets. Planets are numbered starting with 0.
		public int NumPlanets()
		{
			return planets.Count;
		}

		// Returns the planet with the given planet_id. There are NumPlanets()
		// planets. They are numbered starting at 0.
		public Planet GetPlanet(int planetID)
		{
			return planets[planetID];
		}

		// Returns the number of fleets.
		public int NumFleets()
		{
			return fleets.Count;
		}

		// Returns the fleet with the given fleet_id. Fleets are numbered starting
		// with 0. There are NumFleets() fleets. fleet_id's are not consistent from
		// one turn to the next.
		public Fleet GetFleet(int fleetID)
		{
			return fleets[fleetID];
		}

		// Returns a list of all the planets.
		public List<Planet> Planets()
		{
			return planets;
		}

		// Return a list of all the planets owned by the current player. By
		// convention, the current player is always player number 1.
		public List<Planet> MyPlanets()
		{
			List<Planet> r = new List<Planet>();
			foreach (Planet p in planets)
			{
				if (p.Owner() == 1)
				{
					r.Add(p);
				}
			}
			return r;
		}

		// Return a list of all neutral planets.
		public List<Planet> NeutralPlanets()
		{
			List<Planet> r = new List<Planet>();
			foreach (Planet p in planets)
			{
				if (p.Owner() == 0)
				{
					r.Add(p);
				}
			}
			return r;
		}

		// Return a list of all the planets owned by rival players. This excludes
		// planets owned by the current player, as well as neutral planets.
		public List<Planet> EnemyPlanets()
		{
			List<Planet> r = new List<Planet>();
			foreach (Planet p in planets)
			{
				if (p.Owner() >= 2)
				{
					r.Add(p);
				}
			}
			return r;
		}

		// Return a list of all the planets that are not owned by the current
		// player. This includes all enemy planets and neutral planets.
		public List<Planet> NotMyPlanets()
		{
			List<Planet> r = new List<Planet>();
			foreach (Planet p in planets)
			{
				if (p.Owner() != 1)
				{
					r.Add(p);
				}
			}
			return r;
		}

		// Return a list of all the fleets.
		public Fleets Fleets()
		{
			Fleets r = new Fleets();
			foreach (Fleet f in fleets)
			{
				r.Add(f);
			}
			return r;
		}

		// Return a list of all the fleets owned by the current player.
		public Fleets MyFleets()
		{
			Fleets r = new Fleets();
			foreach (Fleet f in fleets)
			{
				if (f.Owner() == 1)
				{
					r.Add(f);
				}
			}
			return r;
		}

		// Return a list of all the fleets owned by enemy players.
		public Fleets EnemyFleets()
		{
			Fleets r = new Fleets();
			foreach (Fleet f in fleets)
			{
				if (f.Owner() != 1)
				{
					r.Add(f);
				}
			}
			return r;
		}

		// Returns the distance between two planets, rounded up to the next highest
		// integer. This is the number of discrete time steps it takes to get
		// between the two planets.
		public int Distance(int sourcePlanet, int destinationPlanet)
		{
			Planet source = planets[sourcePlanet];
			Planet destination = planets[destinationPlanet];
			double dx = source.X() - destination.X();
			double dy = source.Y() - destination.Y();
			return (int)Math.Ceiling(Math.Sqrt(dx * dx + dy * dy));
		}

		// Sends an order to the game engine. An order is composed of a source
		// planet number, a destination planet number, and a number of ships. A
		// few things to keep in mind:
		//   * you can issue many orders per turn if you like.
		//   * the planets are numbered starting at zero, not one.
		//   * you must own the source planet. If you break this rule, the game
		//     engine kicks your bot out of the game instantly.
		//   * you can't move more ships than are currently on the source planet.
		//   * the ships will take a few turns to reach their destination. Travel
		//     is not instant. See the Distance() function for more info.
		public void IssueOrder(int sourcePlanet, int destinationPlanet, int numShips)
		{
			Console.WriteLine("" + sourcePlanet + " " + destinationPlanet + " " + numShips);
			Console.Out.Flush();
		}

		// Sends an order to the game engine. An order is composed of a source
		// planet number, a destination planet number, and a number of ships. A
		// few things to keep in mind:
		//   * you can issue many orders per turn if you like.
		//   * the planets are numbered starting at zero, not one.
		//   * you must own the source planet. If you break this rule, the game
		//     engine kicks your bot out of the game instantly.
		//   * you can't move more ships than are currently on the source planet.
		//   * the ships will take a few turns to reach their destination. Travel
		//     is not instant. See the Distance() function for more info.
		public void IssueOrder(Planet source, Planet dest, int numShips)
		{
			Console.WriteLine("" + source.PlanetID() + " " + dest.PlanetID() +
				" " + numShips);
			Console.Out.Flush();
		}

		public void IssueOrder(Move move)
		{
			Console.WriteLine("" + move.SourceID + " " + move.DestinationID +
				" " + move.NumSheeps);
			Console.Out.Flush();
		}

		// Sends the game engine a message to let it know that we're done sending
		// orders. This signifies the end of our turn.
		public void FinishTurn()
		{
			Console.WriteLine("go");
			Console.Out.Flush();
		}

		// Returns true if the named player owns at least one planet or fleet.
		// Otherwise, the player is deemed to be dead and false is returned.
		public bool IsAlive(int playerID)
		{
			foreach (Planet p in planets)
			{
				if (p.Owner() == playerID)
				{
					return true;
				}
			}
			foreach (Fleet f in fleets)
			{
				if (f.Owner() == playerID)
				{
					return true;
				}
			}
			return false;
		}

		// If the game is not yet over (ie: at least two players have planets or
		// fleets remaining), returns -1. If the game is over (ie: only one player
		// is left) then that player's number is returned. If there are no
		// remaining players, then the game is a draw and 0 is returned.
		public int Winner()
		{
			List<int> remainingPlayers = new List<int>();
			foreach (Planet p in planets)
			{
				if (!remainingPlayers.Contains(p.Owner()))
				{
					remainingPlayers.Add(p.Owner());
				}
			}
			foreach (Fleet f in fleets)
			{
				if (!remainingPlayers.Contains(f.Owner()))
				{
					remainingPlayers.Add(f.Owner());
				}
			}
			switch (remainingPlayers.Count)
			{
				case 0:
					return 0;
				case 1:
					return remainingPlayers[0];
				default:
					return -1;
			}
		}

		// Returns the number of ships that the current player has, either located
		// on planets or in flight.
		public int NumShips(int playerID)
		{
			int numShips = 0;
			foreach (Planet p in planets)
			{
				if (p.Owner() == playerID)
				{
					numShips += p.NumShips();
				}
			}
			foreach (Fleet f in fleets)
			{
				if (f.Owner() == playerID)
				{
					numShips += f.NumShips();
				}
			}
			return numShips;
		}

		// Parses a game state from a string. On success, returns 1. On failure,
		// returns 0.
		private int ParseGameState(string s)
		{
			planets.Clear();
			fleets.Clear();
			int planetID = 0;
			string[] lines = s.Split('\n');
			for (int i = 0; i < lines.Length; ++i)
			{
				string line = lines[i];
				int commentBegin = line.IndexOf('#');
				if (commentBegin >= 0)
				{
					line = line.Substring(0, commentBegin);
				}
				if (line.Trim().Length == 0)
				{
					continue;
				}
				string[] tokens = line.Split(' ');
				if (tokens.Length == 0)
				{
					continue;
				}
				if (tokens[0].Equals("P"))
				{
					if (tokens.Length != 6)
					{
						return 0;
					}
					double x = Double.Parse(tokens[1]);
					double y = Double.Parse(tokens[2]);
					int owner = Int32.Parse(tokens[3]);
					int numShips = Int32.Parse(tokens[4]);
					int growthRate = Int32.Parse(tokens[5]);
					Planet p = new Planet(planetID++,
					                      owner,
					                      numShips,
					                      growthRate,
					                      x, y);
					planets.Add(p);
				}
				else if (tokens[0].Equals("F"))
				{
					if (tokens.Length != 7)
					{
						return 0;
					}
					int owner = Int32.Parse(tokens[1]);
					int numShips = Int32.Parse(tokens[2]);
					int source = Int32.Parse(tokens[3]);
					int destination = Int32.Parse(tokens[4]);
					int totalTripLength = Int32.Parse(tokens[5]);
					int turnsRemaining = Int32.Parse(tokens[6]);
					Fleet f = new Fleet(owner,
					                    numShips,
					                    source,
					                    destination,
					                    totalTripLength,
					                    turnsRemaining);
					fleets.Add(f);
				}
				else
				{
					return 0;
				}
			}
			return 1;
		}

		// Store all the planets and fleets. OMG we wouldn't wanna lose all the
		// planets and fleets, would we!?
		private readonly Planets planets;
		private readonly Fleets fleets;

		public int CompareNumberOfShipsLT(Planet planet1, Planet planet2)
		{
			return (planet1.NumShips() - planet2.NumShips());
		}

		public int CompareNumberOfShipsGT(Planet planet1, Planet planet2)
		{
			return (planet2.NumShips() - planet1.NumShips());
		}

		private static int CompareSecondOfPair(Pair<int, int> pair1, Pair<int, int> pair2)
		{
			return pair2.Second - pair1.Second;
		}

		private static Fleets GetThisTurnFleets(uint turn, IEnumerable<Fleet> thisPlanetFleets)
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

		//Any planets
		public Planets WeakestPlanets(Planets planetList, int number)
		{
			Planets weakestPlanets = new Planets(number);
			if (number == 1)
			{
				Planet weakestPlanet = planetList[0];

				foreach (Planet planet in planetList)
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
				Planets sortedPlanets = planetList;
				sortedPlanets.Sort(CompareNumberOfShipsLT);

				if (number > sortedPlanets.Count)
				{
					number = planetList.Count;
				}

				weakestPlanets = sortedPlanets.GetRange(0, number);
			}
			return weakestPlanets;
		}

		public Planets StrongestPlanets(Planets planetList, int number)
		{
			Planets strongestPlanets = new Planets(number);
			if (number == 1)
			{
				Planet strongestPlanet = planetList[0];

				foreach (Planet planet in planetList)
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
				Planets sortedPlanets = planetList;
				sortedPlanets.Sort(CompareNumberOfShipsGT);

				if (number > sortedPlanets.Count)
				{
					number = planetList.Count;
				}

				strongestPlanets = sortedPlanets.GetRange(0, number);
			}

			return strongestPlanets;
		}

		//My Planets
		public Planets MyWeakestPlanets(int number)
		{
			return WeakestPlanets(MyPlanets(), number);
		}

		public Planets MyStrongestPlanets(int number)
		{
			return StrongestPlanets(MyPlanets(), number);
		}

		//Neutral Planets
		public Planets NeutralWeakestPlanets(int number)
		{
			return WeakestPlanets(NeutralPlanets(), number);
		}

		public Planets NeutralStrongestPlanets(int number)
		{
			return StrongestPlanets(NeutralPlanets(), number);
		}

		//Opponents Planets
		public Planets EnemyWeakestPlanets(int number)
		{
			return WeakestPlanets(EnemyPlanets(), number);
		}

		public Planets EnemyStrongestPlanets(int number)
		{
			return StrongestPlanets(EnemyPlanets(), number);
		}

		public Planets PlanetsWithGivenOwner(Planets planetList, int ownerID)
		{
			if (ownerID == -1)
			{
				return planetList;
			}

			Planets selectedPlanets = new Planets();

			foreach (Planet planet in planetList)
			{
				if (planet.Owner() == ownerID)
				{
					selectedPlanets.Add(planet);
				}
			}
			return selectedPlanets;
		}

		public Fleets FleetsWithGivenOwner(Fleets fleetList, int ownerID)
		{
			if (ownerID == -1)
			{
				return fleetList;
			}
			Fleets selectedFleets = new Fleets();

			foreach (Fleet fleet in fleetList)
			{
				if (fleet.Owner() == ownerID)
				{
					selectedFleets.Add(fleet);
				}
			}
			return selectedFleets;
		}

		public Fleets FleetsGoingToPlanet(Fleets fleetList, Planet planet)
		{
			Fleets attackingFleets = new Fleets();
			foreach (Fleet fleet in fleetList)
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
			return FleetsGoingToPlanet(MyFleets(), planet);
		}

		public Fleets EnemyFleetsGoingToPlanet(Planet planet)
		{
			return FleetsGoingToPlanet(EnemyFleets(), planet);
		}

		private Planets PlanetsUnderAttack(int ownerID)
		{
			Fleets enemyFleets = EnemyFleets();
			Planets attackedPlanets = new Planets();

			foreach (Fleet enemyFleet in enemyFleets)
			{
				Planet planet = GetPlanet(enemyFleet.DestinationPlanet());
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

		public Planets PlanetsWithinProximityToPlanet(Planets planetList, Planet thisPlanet, int proximityTreshold)
		{
			Planets nearbyPlanets = new Planets();

			foreach (Planet planet in planetList)
			{
				if (planet.PlanetID() == thisPlanet.PlanetID())
				{
					continue;
				}
				int distance = Distance(planet.PlanetID(), thisPlanet.PlanetID());
				if (distance <= proximityTreshold)
				{
					nearbyPlanets.Add(planet);
				}
			}
			return nearbyPlanets;
		}

		public Planets MyPlanetsWithinProximityToPlanet(Planet thisPlanet, int proximityTreshold)
		{
			return PlanetsWithinProximityToPlanet(MyPlanets(), thisPlanet, proximityTreshold);
		}

		public Planets NeutralPlanetsWithinProximityToPlanet(Planet thisPlanet, int proximityTreshold)
		{
			return PlanetsWithinProximityToPlanet(NeutralPlanets(), thisPlanet, proximityTreshold);
		}

		public Planets EnemyPlanetsWithinProximityToPlanet(Planet thisPlanet, int proximityTreshold)
		{
			return PlanetsWithinProximityToPlanet(EnemyPlanets(), thisPlanet, proximityTreshold);
		}

		public Planet PlanetFutureStatus(Planet planet, int numberOfTurns)
		{
			Planet planetInFuture = new Planet(planet);

			// All fleets heading to this planet
			Fleets thisPlanetFleets = FleetsGoingToPlanet(Fleets(), planet);

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

		public Planets MyEndangeredPlanets(int numberOfTurns, int treshold)
		{
			Planets allMyPlanets = MyPlanets();
			Planets endangeredPlanets = new Planets();

			foreach (Planet planet in allMyPlanets)
			{
				Planet planetInFuture = PlanetFutureStatus(planet, numberOfTurns);
				if ((planetInFuture.Owner() != 1) || (planetInFuture.NumShips() <= treshold))
				{
					endangeredPlanets.Add(planet);
				}
			}

			return endangeredPlanets;
		}

		public int GetPlanetSummaryDistance(Planets planetList, Planet thisPlanet)
		{
			int distance = 0;
			foreach (Planet planet in planetList)
			{
				if (planet.PlanetID() == thisPlanet.PlanetID())
				{
					continue;
				}
				distance += Distance(planet.PlanetID(), thisPlanet.PlanetID());
			}
			return distance;
		}

		public int CompareImportanceOfPlanetsGT(Planet planet1, Planet planet2)
		{
			if (planet1.PlanetID() == planet2.PlanetID()) return 0;

			Planets myPlanets = MyPlanets();

			int growthDifference =
				(planet1.GrowthRate() -
				 planet2.GrowthRate())
				*GROWS_RATE_KOEF;
			int distanceDifference = 
				(GetPlanetSummaryDistance(myPlanets, planet1) -
				 GetPlanetSummaryDistance(myPlanets, planet2))
				*DISTANCE_KOEF;

			return growthDifference + distanceDifference;
		}

		public Planets MostImportantPlanets(Planets planetList, int number)
		{
			Planets mostImportantPlanets = new Planets(number);
			if (number == 1)
			{
				Planet mostImportantPlanet = planetList[0];

				foreach (Planet planet in planetList)
				{
					if (CompareImportanceOfPlanetsGT(planet, mostImportantPlanet) > 0)
					{
						mostImportantPlanet = planet;
					}
				}
				mostImportantPlanets.Add(mostImportantPlanet);
			}
			else if (number != 0)
			{
				Planets sortedPlanets = planetList;
				sortedPlanets.Sort(CompareImportanceOfPlanetsGT);

				if (number > sortedPlanets.Count)
				{
					number = planetList.Count;
				}

				mostImportantPlanets = sortedPlanets.GetRange(0, number);
			}

			return mostImportantPlanets;
		}

		public int GetFleetsShipNum(Fleets fleetList)
		{
			int num = 0;
			foreach (Fleet fleet in fleetList)
			{
				num += fleet.NumShips();
			}
			return num;
		}

		internal int GetClosestFleetDistance(Fleets fleetList)
		{
			int distance = int.MaxValue;
			foreach (Fleet fleet in fleetList)
			{
				if (fleet.TurnsRemaining() < distance)
				{
					distance = fleet.TurnsRemaining();
				}
			}
			return distance;
		}
	}
}
