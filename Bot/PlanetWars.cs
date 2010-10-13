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
using PlanetHolders = System.Collections.Generic.List<Bot.PlanetHolder>;

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
			Router.Init(planets);
			planetHolders = new PlanetHolders(planets.Count);
			foreach (Planet planet in planets)
			{
				PlanetHolder planetHolder = new PlanetHolder(planet, FleetsGoingToPlanet(Fleets(), planet));
				planetHolders.Add(planetHolder);
			}
			//FillMyPlanetsFrontLevel();
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

		public PlanetHolder GetPlanetHolder(int planetID)
		{
			return planetHolders[planetID];
		}

		public PlanetHolder GetPlanetHolder(Planet planet)
		{
			return planetHolders[planet.PlanetID()];
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

		public PlanetHolders PlanetHolders()
		{
			return planetHolders;
		}

		// Return a list of all the planets owned by the current player. By
		// convention, the current player is always player number 1.
		public Planets MyPlanets()
		{
			Planets myPlanets = new Planets();
			foreach (Planet p in planets)
			{
				if (p.Owner() == 1)
				{
					myPlanets.Add(p);
				}
			}
			return myPlanets;
		}

		public PlanetHolders MyPlanetHolders()
		{
			return PlanetHoldersWithGivenOwner(1);
		}

		public PlanetHolders EnemyPlanetHolders()
		{
			return PlanetHoldersWithGivenOwner(2);
		}

		public PlanetHolders NeutralPlanetHolders()
		{
			return PlanetHoldersWithGivenOwner(0);
		}

		public PlanetHolders PlanetHoldersWithGivenOwner(int owner)
		{
			PlanetHolders myPlanetHolders = new PlanetHolders();
			foreach (PlanetHolder p in planetHolders)
			{
				if (p.Owner() == owner)
				{
					myPlanetHolders.Add(p);
				}
			}
			return myPlanetHolders;
		}

		// Return a list of all neutral planets.
		public Planets NeutralPlanets()
		{
			Planets neutralPlanets = new Planets();
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
		public Planets EnemyPlanets()
		{
			Planets enemyPlanets = new Planets();
			foreach (Planet p in planets)
				if (p.Owner() >= 2) enemyPlanets.Add(p);
			return enemyPlanets;
		}

		// Return a list of all the planets that are not owned by the current
		// player. This includes all enemy planets and neutral planets.
		public Planets NotMyPlanets()
		{
			Planets notMyPlanets = new Planets();
			foreach (Planet p in planets)
			{
				if (p.Owner() != 1)
					notMyPlanets.Add(p);
			}
			return notMyPlanets;
		}

		// Return a list of all the fleets.
		public Fleets Fleets()
		{
			Fleets r = new Fleets();
			foreach (Fleet f in fleets)
				r.Add(f);
			return r;
		}

		// Return a list of all the fleets owned by the current player.
		public Fleets MyFleets()
		{
			Fleets r = new Fleets();
			foreach (Fleet f in fleets)
			{
				if (f.Owner() == 1)
					r.Add(f);
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
					r.Add(f);
			}
			return r;
		}

		// Returns the distance between two planets, rounded up to the next highest
		// integer. This is the number of discrete time steps it takes to get
		// between the two planets.
		public int Distance(int sourcePlanet, int destinationPlanet)
		{
			return Distance(GetPlanet(sourcePlanet), GetPlanet(destinationPlanet));
			//return Router.Distance(sourcePlanet, destinationPlanet);
		}

		public int Distance(Planet source, Planet destination)
		{
			double dx = source.X() - destination.X();
			double dy = source.Y() - destination.Y();
			double squared = dx * dx + dy * dy;
			double rooted = Math.Sqrt(squared);
			int result = (int)Math.Ceiling(rooted);
			return result;
			//return Router.Distance(source, destination);
		}

		public bool IsValid(int sourcePlanetID, int destPlanetID, int numShips)
		{
			Planet source = GetPlanet(sourcePlanetID);
			Planet dest = GetPlanet(destPlanetID);
			return IsValid(source, dest, numShips);
		}

		public static bool IsValid(Planet source, Planet dest, int numShips)
		{
			if (source.Owner() != 1) return false;
			if (numShips > source.NumShips()) return false;
			if (source.PlanetID() == dest.PlanetID()) return false;
			return true;
		}

		public bool IsValid(Move move)
		{
			return IsValid(move.SourceID, move.DestinationID, move.NumSheeps);
		}

		/*public void IssueOrder(int sourcePlanet, int destinationPlanet, int numShips)
		{
			Move move = new Move(sourcePlanet, destinationPlanet, numShips);
			IssueOrder(move);
		}

		public void IssueOrder(Planet source, Planet dest, int numShips)
		{
			Move move = new Move(source.PlanetID(), dest.PlanetID(), numShips);
			IssueOrder(move);
		}*/

		public void IssueOrder(Move move)
		{
			if (!IsValid(move))
			{
			#if DEBUG
				Logger.Log("  !Invalid move: from " + move.SourceID + " to " + move.DestinationID + " num " + move.NumSheeps);
			#endif
				return;
			}

			if (move.TurnsBefore == 0)
			{
				Console.WriteLine("" + move.SourceID + " " + move.DestinationID + " " + move.NumSheeps);
				Console.Out.Flush();
			}
#if DEBUG
			//Logger.Log("" + move.SourceID + " " + move.DestinationID +" " + move.NumSheeps);
#endif

			MakeMove(move);
		}

		private void MakeMove(Move move)
		{
			fleets.Add(MoveToFleet(1, move));
			GetPlanet(move.SourceID).RemoveShips(move.NumSheeps);
		}

		// Sends the game engine a message to let it know that we're done sending
		// orders. This signifies the end of our turn.
		public void FinishTurn()
		{
			Console.WriteLine("go");
			Console.Out.Flush();
#if DEBUG
			Logger.Log("go");
#endif
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
		private Fleets fleets;
		private readonly PlanetHolders planetHolders;

		public Fleets GetThisTurnFleets(int turn, IEnumerable<Fleet> thisPlanetFleets)
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

		/*private static void BattleForPlanet(Planet planetInFuture, List<Pair<int, int>> ships)
		{
			// Were there any fleets other than the one on the planet?
			if (ships.Count > 1)
			{
				// Sorts the fleets in descending order by the number of ships in the fleet
				ships.Sort(Pair<int, int>.CompareSecondOfPair);

				Pair<int, int> winner = ships[0];
				Pair<int, int> secondToWinner = ships[1];

				if (winner.Second == secondToWinner.Second)
				{
					//old owner stays
					//planetInFuture.Owner(0);
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
		}*/

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

		public static Fleets FleetsWithGivenOwner(Fleets fleetList, int ownerID)
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
				if (planet.Owner() == ownerID && attackedPlanets.IndexOf(planet) == -1) attackedPlanets.Add(planet);
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

		/*public Planets MyInvasionNeutralPlanetsUnderAttack()
		{
			Planets neutralPlanetsUnderAttack = NeutralPlanetsUnderAttack();
			Planets myInvasionNeutralPlanetsUnderAttack = new Planets();
			Fleets myFleets = MyFleets();
			foreach (Fleet fleet in myFleets)
			{
				Planet planet = GetPlanet(fleet.DestinationPlanet());
				if (neutralPlanetsUnderAttack.IndexOf(planet) != -1 &&
					myInvasionNeutralPlanetsUnderAttack.IndexOf(planet) == -1)
				{
					myInvasionNeutralPlanetsUnderAttack.Add(planet);
				}
			}
			return myInvasionNeutralPlanetsUnderAttack;
		}*/

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
			return GetPlanetHolder(planet).GetFutureState(numberOfTurns);
		}

		public Planets MyEndangeredPlanets()
		{
			PlanetHolders myPlanetHolders = PlanetHolders(); //MyPlanetHolders();
			Planets endangeredPlanets = new Planets();

			foreach (PlanetHolder planetHolder in myPlanetHolders)
			{
				if (planetHolder.GetOwnerSwitchesFromMyToEnemy().Count > 0)
				{
					endangeredPlanets.Add(planetHolder.GetPlanet());
				}
			}

			return endangeredPlanets;
		}

		public int GetPlanetSummaryDistance(Planets planetList, Planet thisPlanet)
		{
			int distance = 0;
			foreach (Planet planet in planetList)
			{
				if (planet.PlanetID() == thisPlanet.PlanetID()) continue;
				distance += Distance(planet, thisPlanet);
			}
			return distance;
		}

		public int GetFleetsShipNum(Fleets fleetList)
		{
			return GetFleetsShipNumCloserThan(fleetList, 0);
		}

		private static int GetFleetsShipNumWithCondition(Fleets fleetList, int treshold, int sign)
		{
			int num = 0;
			foreach (Fleet fleet in fleetList)
			{
				if ((treshold == 0) ||
					(((fleet.TurnsRemaining() <= treshold) && (sign < 0)) ||
					((fleet.TurnsRemaining() > treshold) && (sign > 0))))
					num += fleet.NumShips();
			}
			return num;
		}

		public int GetFleetsShipNumCloserThan(Fleets fleetList, int treshold)
		{
			return GetFleetsShipNumWithCondition(fleetList, treshold, -1);
		}

		public int GetFleetsShipNumFarerThan(Fleets fleetList, int treshold)
		{
			return GetFleetsShipNumWithCondition(fleetList, treshold, 1);
		}

		internal static int GetClosestFleetDistance(Fleets fleetList)
		{
			return GetLimitFleetDistance(fleetList, -1);
		}

		internal static int GetFarestFleetDistance(Fleets fleetList)
		{
			return GetLimitFleetDistance(fleetList, 1);
		}

		private static int GetLimitFleetDistance(Fleets fleetList, int limitType)
		{
			int distance = Math.Sign(limitType) > 0 ? 0 : int.MaxValue;
			foreach (Fleet fleet in fleetList)
			{
				if (Math.Sign(fleet.TurnsRemaining() - distance) == Math.Sign(limitType))
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

		public List<Step> GetMyPlanetSaveSteps(Planet planet)
		{
			List<Step> saveSteps = new List<Step>();

			PlanetHolder planetHolder = GetPlanetHolder(planet);

			List<PlanetOwnerSwitch> switches = planetHolder.GetOwnerSwitchesFromMyToEnemy();
			if (switches.Count == 0) return saveSteps;

			//Save from closest danger. From next dangers we will find steps on next turns
			int turn = switches[0].TurnsBefore;

			Step step = null;
			Planet planetInFuture = planetHolder.GetFutureState(turn);
			if (planetInFuture.Owner() != 1)
			{
				step = new Step(0, turn, planetInFuture.NumShips() + Config.MinShipsOnPlanetsAfterDefend);
			}
			else if (planetInFuture.NumShips() < Config.MinShipsOnPlanetsAfterDefend)
			{
				step = new Step(0, turn, Config.MinShipsOnPlanetsAfterDefend - planetInFuture.NumShips());
			}

			if (step != null) saveSteps.Add(step);

			return saveSteps;

		}

		public int CanSend(Planet planet)
		{
			if (planet.Owner() != 1) return 0;

			return GetPlanetHolder(planet).CanSend();
		}

		public int CanSend(Planet planet, int turn)
		{
			return GetPlanetHolder(planet).CanSend(turn);
		}

		//# Returns a string representation of the entire game state.
		public static string SerializeGameState(List<Planet> planets, List<Fleet> fleets, bool forDebug)
		{
			string message = "";
			int n = 0;
			foreach (Planet p in planets)
			{
				message += 
					(forDebug ? "\"" : "") + 
					SerializePlanet(p) + 
					"#" + 
					n++ + 
					(forDebug ? "\\n\" +" : "") +
					"\n";
			}
			message += "\n";

			foreach (Fleet f in fleets)
			{
				message +=
					(forDebug ? "\"" : "") + 
					SerializeFleet(f) +
					(forDebug ? "\\n\" +" : "") +
					"\n";
			}

			message +=
				//"\n" + 
				(forDebug ? "\"" : "") +
				"go" +
				(forDebug ? "\\n\"" : "")+
				"\n";
			message = message.Replace("\n\n", "\n");
			return message;
		}

		public static string SerializeGameState(PlanetWars pw, bool forDebug)
		{
			return SerializeGameState(pw.Planets(), pw.Fleets(), forDebug);
		}

		public static string SerializeGameState(List<Planet> planets, List<Fleet> fleets)
		{
			return SerializeGameState(planets, fleets, false);
		}

		//# Generates a string representation of a planet. This is used to send data
		//# about the planets to the client programs.
		public static string SerializePlanet(Planet planet)
		{
			int owner = planet.Owner();
			string message = 
				"P " + 
				string.Format("{0:R}", planet.X()) + 
				" " + 
				string.Format("{0:R}", planet.Y()) + 
				" " + 
				owner +
				" " + 
				planet.NumShips() + 
				" " + 
				planet.GrowthRate();
			return message.Replace(".0 ", " ");
		}

		//# Generates a string representation of a fleet. This is used to send data
		//# about the fleets to the client programs.
		public static string SerializeFleet(Fleet fleet)
		{
			int owner = fleet.Owner();
			string message = 
				"F " + 
				owner + 
				" " + 
				fleet.NumShips() + 
				" " +
				fleet.SourcePlanet() + 
				" " + 
				fleet.DestinationPlanet() + 
				" " +
				fleet.TotalTripLength() + 
				" " + 
				fleet.TurnsRemaining();
			return message.Replace(".0 ", " ");
		}

		public int GetClosestEnemyPlanetDistance(Planet planet)
		{
			return GetClosestPlanetDistance(planet, EnemyPlanets());
		}

		public int GetClosestMyPlanetDistance(Planet planet)
		{
			return GetClosestPlanetDistance(planet, MyPlanets());
		}

		public int GetClosestPlanetDistance(Planet planet, Planets planetList)
		{
			int distance = int.MaxValue;
			foreach (Planet eachPlanet in planetList)
			{
				int currentDistance = Distance(planet, eachPlanet);
				if (distance > currentDistance) distance = currentDistance;
			}
			return distance;
		}

		public int GetPlanetsShipNum(Planets planetList)
		{
			int num = 0;
			foreach (Planet planet in planetList)
			{
				num += planet.NumShips();
			}
			return num;
		}

		public int EnemyCanSend(Planet planet, int passTurns)
		{
			if (planet.Owner() != 2) return 0;

			//TODO make more intelligent strategy
			int canSend = planet.NumShips() + passTurns * planet.GrowthRate();
			Fleets myFleets = MyFleetsGoingToPlanet(planet);
			foreach (Fleet myFleet in myFleets)
			{
				if (myFleet.TurnsRemaining() <= passTurns) canSend -= myFleet.NumShips();
			}

			Fleets enemyFleets = EnemyFleetsGoingToPlanet(planet);
			foreach (Fleet enemyFleet in enemyFleets)
			{
				if (enemyFleet.TurnsRemaining() <= passTurns) canSend += enemyFleet.NumShips();
			}

			return canSend < 0 ? 0 : canSend;
		}

		public Moves GetPossibleDefendMoves(Planet defendPlanet, Planets planetList, int numTurns)
		{
			Moves moves = new Moves();

			foreach (Planet planet in planetList)
			{
				if (planet == defendPlanet) continue;

				int distance = Distance(defendPlanet, planet);

				int moveTurn = numTurns - distance;
				if (moveTurn < 0) continue;

				int canSend = EnemyCanSend(planet, moveTurn);
				if (canSend > 0)
				{
					Move move = new Move(planet.PlanetID(), defendPlanet.PlanetID(), canSend);
					move.TurnsBefore = moveTurn;

					moves.Add(move);
				}
			}

			return moves;
		}

		public Fleet MoveToFleet(int owner, Move move)
		{
			int distance = Distance(move.SourceID, move.DestinationID) + move.TurnsBefore;
			Fleet fleet = new Fleet(
				owner,
				move.NumSheeps,
				move.SourceID,
				move.DestinationID,
				distance,
				distance
				);
			return fleet;
		}

		private int [,] enemyAid;
		public int GetEnemyAid(Planet planet, int numberOfTurns)
		{
			if (enemyAid == null)
			{
				int iSize = Planets().Count;
				int jSize = Router.MaxDistance + 1;
				enemyAid = new int[iSize, jSize];
				for (int i = 0; i < iSize; i++)
				{
					for (int j = 0; j < jSize; j++)
					{
						enemyAid[i, j] = -1;
					}
				}
			}

			if (numberOfTurns > Router.MaxDistance) return GetEnemyAid(planet, Router.MaxDistance);
			if (enemyAid[planet.PlanetID(), numberOfTurns] != -1) return enemyAid[planet.PlanetID(), numberOfTurns];

			enemyAid[planet.PlanetID(), numberOfTurns] = 0;

			/*Fleets enemyFleets = FleetsGoingToPlanet(EnemyFleets(), planet);
			foreach (Fleet enemyFleet in enemyFleets)
			{
				if (enemyFleet.TurnsRemaining() > numberOfTurns) continue;
				enemyAid[planet.PlanetID(), numberOfTurns] += enemyFleet.NumShips();
			}*/

			Planets enemyPlanets = Planets();
			foreach (Planet enemyPlanet in enemyPlanets)
			{
				if (enemyPlanet.PlanetID() == planet.PlanetID()) continue;

				int distance = Distance(enemyPlanet, planet);
				if (distance > numberOfTurns) continue;

				int sendTurn = numberOfTurns - distance;

				Planet futurePlanet = PlanetFutureStatus(enemyPlanet, sendTurn);
				if (futurePlanet.Owner() < 2) continue;

				int numShips = futurePlanet.NumShips();
				enemyAid[planet.PlanetID(), numberOfTurns] += numShips;
			}
			return enemyAid[planet.PlanetID(), numberOfTurns];
		}

		public void IssueOrder(MovesSet movesSet)
		{
			foreach (Move move in movesSet.Moves)
			{
				IssueOrder(move);
			}
		}

		public double AverageMovesDistance(Moves moves)
		{
			if (moves.Count == 0) return 0;

			double sumDistance = 0;
			foreach (Move move in moves)
			{
				sumDistance += Distance(move.SourceID, move.DestinationID);
			}
			return sumDistance/moves.Count;
		}
	}
}
