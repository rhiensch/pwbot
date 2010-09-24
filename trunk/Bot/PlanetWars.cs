// Contestants do not need to worry about anything in this file. This is just
// helper code that does the boring stuff for you, so you can focus on the
// interesting stuff. That being said, you're welcome to change anything in
// this file if you know what you're doing.

#define DEBUG

using System;
using System.Collections.Generic;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class PlanetWars
	{
		// Constructs a PlanetWars object instance, given a string containing a
		// description of a game state.
		public PlanetWars(string gameStatestring)
		{
			planets = new List<Planet>(); 
			fleets = new Fleets();
			ParseGameState(gameStatestring);
			FillMyPlanetsFrontLevel();
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
		public Planets Planets()
		{
			return planets;
		}

		// Return a list of all the planets owned by the current player. By
		// convention, the current player is always player number 1.
		private Planets myPlanets;
		public Planets MyPlanets()
		{
			if (myPlanets != null) return myPlanets;
			myPlanets = new Planets();
			foreach (Planet p in planets)
			{
				if (p.Owner() == 1)
				{
					myPlanets.Add(p);
				}
			}
			return myPlanets;
		}

		// Return a list of all neutral planets.
		private Planets neutralPlanets;
		public Planets NeutralPlanets()
		{
			if (neutralPlanets != null) return neutralPlanets;
			neutralPlanets = new Planets();
			foreach (Planet p in planets)
			{
				if (p.Owner() == 0)
				{
					neutralPlanets.Add(p);
				}
			}
			return neutralPlanets;
		}

		// Return a list of all the planets owned by rival players. This excludes
		// planets owned by the current player, as well as neutral planets.
		private Planets enemyPlanets;
		public Planets EnemyPlanets()
		{
			if (enemyPlanets != null) return enemyPlanets;
			enemyPlanets = new Planets();
			foreach (Planet p in planets)
			{
				if (p.Owner() >= 2)
				{
					enemyPlanets.Add(p);
				}
			}
			return enemyPlanets;
		}

		// Return a list of all the planets that are not owned by the current
		// player. This includes all enemy planets and neutral planets.
		private Planets notMyPlanets;
		public Planets NotMyPlanets()
		{
			if (notMyPlanets != null) return notMyPlanets;
			notMyPlanets = new Planets();
			foreach (Planet p in planets)
			{
				if (p.Owner() != 1)
				{
					notMyPlanets.Add(p);
				}
			}
			return notMyPlanets;
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
			return Distance(source, destination);
		}

		public int Distance(Planet source, Planet destination)
		{
			double dx = source.X() - destination.X();
			double dy = source.Y() - destination.Y();
			double squared = dx * dx + dy * dy;
			double rooted = Math.Sqrt(squared);
			return (int)Math.Ceiling(rooted);

			/*double dx = source.X() - destination.X();
			double dy = source.Y() - destination.Y();
			return (int)Math.Ceiling(Math.Sqrt(dx * dx + dy * dy));*/
		}

		private bool IsValid(int sourcePlanetID, int destPlanetID, int numShips)
		{
			Planet source = GetPlanet(sourcePlanetID);
			Planet dest = GetPlanet(destPlanetID);
			return IsValid(source, dest, numShips);
		}

		private bool IsValid(Planet source, Planet dest, int numShips)
		{
			if (source.Owner() != 1) return false;
			if (numShips > source.NumShips()) return false;
			if (source.PlanetID() == dest.PlanetID()) return false;
			return true;
		}

		private bool IsValid(Move move)
		{
			return IsValid(move.SourceID, move.DestinationID, move.NumSheeps);
		}

		public void IssueOrder(int sourcePlanet, int destinationPlanet, int numShips)
		{
			Move move = new Move(sourcePlanet, destinationPlanet, numShips);
			IssueOrder(move);
		}

		public void IssueOrder(Planet source, Planet dest, int numShips)
		{
			Move move = new Move(source.PlanetID(), dest.PlanetID(), numShips);
			IssueOrder(move);
		}

		public void IssueOrder(Move move)
		{
			if (!IsValid(move))
			{
			#if DEBUG
				Logger.Log("  !Invalid move: from " + move.SourceID + " to " + move.DestinationID + " num " + move.NumSheeps);
			#endif
				return;
			}

			Console.WriteLine("" + move.SourceID + " " + move.DestinationID +
				" " + move.NumSheeps);
			Console.Out.Flush();

			MakeMove(move);
		}

		private void MakeMove(Move move)
		{
			int distance = Distance(move.SourceID, move.DestinationID);
			Fleet newFleet = new Fleet(
				1,
				move.NumSheeps,
				move.SourceID,
				move.DestinationID,
				distance,
				distance);

			fleets.Add(newFleet);
			GetPlanet(move.SourceID).NumShips(GetPlanet(move.SourceID).NumShips() - move.NumSheeps);
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
		private void ParseGameState(string s)
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
						throw new ArgumentException("Planet must have 6 parameters (actual: " +
							Convert.ToString(tokens.Length) + ")");
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
						throw new ArgumentException("Fleet must have 7 parameters (actual: " +
							Convert.ToString(tokens.Length) + ")");
					}
					int owner = Int32.Parse(tokens[1]);
					int numShips = Int32.Parse(tokens[2]);
					int source = Int32.Parse(tokens[3]);
					int destination = Int32.Parse(tokens[4]);
					int totalTripLength = Int32.Parse(tokens[5]);
					int turnsRemaining = Int32.Parse(tokens[6]);
					if (numShips > 0)
					{
						Fleet f = new Fleet(owner,
						                    numShips,
						                    source,
						                    destination,
						                    totalTripLength,
						                    turnsRemaining);
						fleets.Add(f);
					}
				}
			}
		}

		// Store all the planets and fleets. OMG we wouldn't wanna lose all the
		// planets and fleets, would we!?
		private readonly Planets planets;
		private readonly Fleets fleets;

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

		private void BattleForPlanet(Planet planetInFuture, List<Pair<int, int>> ships)
		{
			// Were there any fleets other than the one on the planet?
			if (ships.Count > 1)
			{
				// Sorts the fleets in descending order by the number of ships in the fleet
				ships.Sort(new Comparer(this).CompareSecondOfPair);

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
				sortedPlanets.Sort(new Comparer(this).CompareNumberOfShipsLT);

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
				sortedPlanets.Sort(new Comparer(this).CompareNumberOfShipsGT);

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
				int distance = Distance(planet, thisPlanet);
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
				distance += Distance(planet, thisPlanet);
			}
			return distance;
		}

		public Planets MostImportantPlanets(Planets planetList, int number)
		{
			Comparer comparer = new Comparer(this);
			Planets mostImportantPlanets = new Planets(number);
			if (number == 1)
			{
				Planet mostImportantPlanet = planetList[0];

				foreach (Planet planet in planetList)
				{
					if (comparer.CompareImportanceOfPlanetsGT(planet, mostImportantPlanet) > 0)
					{
						mostImportantPlanet = planet;
					}
				}
				mostImportantPlanets.Add(mostImportantPlanet);
			}
			else if (number != 0)
			{
				Planets sortedPlanets = planetList;
				sortedPlanets.Sort(comparer.CompareImportanceOfPlanetsGT);

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

		private int myProduction = -1;
		private int enemyProduction = -1;

		public int MyProduction
		{
			get
			{
				if (myProduction == -1)
				{
					myProduction = CalcProduction(1);
				}
				return myProduction;
			}
		}

		public int EnemyProduction
		{
			get
			{
				if (enemyProduction == -1)
				{
					enemyProduction = CalcProduction(2);
				}
				return enemyProduction;
			}
		}

		private int CalcProduction(int playerID)
		{
			if (playerID == 0) return 0;
			int production = 0;
			foreach (Planet planet in planets)
			{
				if (planet.Owner() == playerID) production += planet.GrowthRate();
			}
			return production;
		}

		public int Production(int playerID)
		{
			switch (playerID)
			{
				case 1:
					return MyProduction;
				case 2:
					return EnemyProduction;
				default:
					return 0;
			}
		}

		private int myTotalShipCount = -1;
		private int enemyTotalShipCount = -1;

		public int MyTotalShipCount
		{
			get
			{
				if (myTotalShipCount == -1)
				{
					myTotalShipCount = CalcTotalShipCount(1);
				}
				return myTotalShipCount;
			}
		}

		public int EnemyTotalShipCount
		{
			get
			{
				if (enemyProduction == -1)
				{
					enemyTotalShipCount = CalcTotalShipCount(2);
				}
				return enemyTotalShipCount;
			}
		}

		private int CalcTotalShipCount(int playerID)
		{
			int shipCount = 0;
			foreach (Planet planet in planets)
			{
				if (planet.Owner() == playerID) shipCount += planet.NumShips();
			}
			if (playerID > 0)
			{
				foreach (Fleet fleet in fleets)
				{
					if (fleet.Owner() == playerID) shipCount += fleet.NumShips();
				}
			}
			return shipCount;
		}

		public int TotalShipCount(int playerID)
		{
			switch (playerID)
			{
				case 1:
					return MyTotalShipCount;
				case 2:
					return EnemyTotalShipCount;
				default:
					return CalcTotalShipCount(0);
			}
		}

		public Planets FrontPlanets { get; private set; }
		public Planets NotFrontPlanets { get; private set; }
		
		private void FillMyPlanetsFrontLevel()
		{
			if (FrontPlanets == null) FrontPlanets = new Planets(); else FrontPlanets.Clear();
			if (NotFrontPlanets == null) NotFrontPlanets = new Planets(); else NotFrontPlanets.Clear();
			foreach (Planet planet in planets)
			{
				planet.FrontLevel = 0;
			}

			foreach (Planet enemyPlanet in EnemyPlanets())
			{
				Planets nearFrontPlanets = PlanetsWithinProximityToPlanet(MyPlanets(), enemyPlanet, Config.InvokeDistanceForFront);
				foreach (Planet frontPlanet in nearFrontPlanets)
				{
					FrontPlanets.Add(frontPlanet);
					frontPlanet.FrontLevel += 10 + (Config.InvokeDistanceForFront = Distance(enemyPlanet, frontPlanet));
				}
			}

			foreach (Planet myPlanet in MyPlanets())
			{
				if (myPlanet.FrontLevel > 0) continue;
				
				Planets nearPlanets = PlanetsWithinProximityToPlanet(MyPlanets(), myPlanet, Config.InvokeDistanceForFront);
				foreach (Planet nearPlanet in nearPlanets)
				{
					if (nearPlanet.FrontLevel > 0) myPlanet.FrontLevel += nearPlanet.FrontLevel / 10;
				}
				NotFrontPlanets.Add(myPlanet);
			}
		}
	}
}