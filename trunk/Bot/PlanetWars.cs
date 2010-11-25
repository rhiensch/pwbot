// Contestants do not need to worry about anything in this file. This is just
// helper code that does the boring stuff for you, so you can focus on the
// interesting stuff. That being said, you're welcome to change anything in
// this file if you know what you're doing.

#define LOG

using System;
using System.Collections.Generic;
using System.Linq;
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
			//planets.Sort(new Comparer(this).Coordinates);

			Planets allPlanets = Planets();
			Router.Init(allPlanets);
			planetHolders = new PlanetHolders(allPlanets.Count);
			foreach (Planet planet in allPlanets)
			{
				PlanetHolder planetHolder = new PlanetHolder(planet, FleetsGoingToPlanet(Fleets(), planet));
				planetHolders.Add(planetHolder);
			}
			//FillMyPlanetsFrontLevel();
		}

		// Returns the number of planets. Planets are numbered starting with 0.
		public int NumPlanets()
		{
			return Planets().Count;
		}

		// Returns the planet with the given planet_id. There are NumPlanets()
		// planets. They are numbered starting at 0.
		public Planet GetPlanet(int planetID)
		{
			Planets allPlanets = Planets();
			foreach (Planet planet in allPlanets)
			{
				if (planet.PlanetID() == planetID) return planet;
			}
			return null;
			//return planets[planetID];
		}

		public PlanetHolder GetPlanetHolder(int planetID)
		{
			foreach (PlanetHolder planetHolder in planetHolders)
			{
				if (planetHolder.GetPlanet().PlanetID() == planetID) return planetHolder;
			}
			return null;
			//return planetHolders[planetID];
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
			Planets myPlanets = new Planets(Config.MaxPlanets);
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
			Planets neutralPlanets = new Planets(Config.MaxPlanets);
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
			Planets enemyPlanets = new Planets(Config.MaxPlanets);
			foreach (Planet p in planets)
				if (p.Owner() >= 2) enemyPlanets.Add(p);
			return enemyPlanets;
		}

		public Planets EnemyPlanets(int turn)
		{
			Planets enemyPlanets = new Planets(Config.MaxPlanets);
			foreach (Planet p in planets)
			{
				Planet futurePlanet = PlanetFutureStatus(p, turn);
				if (futurePlanet.Owner() >= 2) enemyPlanets.Add(p);
			}
			return enemyPlanets;
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
			return Router.Distance(source, destination);
		}

		public bool IsValid(int sourcePlanetID, int destPlanetID, int numShips)
		{
			Planet source = GetPlanet(sourcePlanetID);
			Planet dest = GetPlanet(destPlanetID);
			return IsValid(source, dest, numShips);
		}

		public bool IsValid(Planet source, Planet dest, int numShips)
		{
			if (source.Owner() != 1)
			{
				//Logger.Log("InValid : not my planet: source = " + source + "    Move: dest = " + dest + " num = " + numShips);
				return false;
			}
			if (source.PlanetID() == dest.PlanetID())
			{
				//Logger.Log("InValid : source = dest: source = " + source + "    Move: dest = " + dest + " num = " + numShips);
				return false;
			}
			if (numShips > source.NumShips())
			{
				//Logger.Log("InValid : > numShips: source = " + source + "    Move: dest = " + dest + " num = " + numShips);
				return false;
			}
			if (numShips > CanSendByPlanets(source, dest))
			{
				//Logger.Log("InValid : > canSend: source = " + source + "    Move: dest = " + dest + " num = " + numShips + " canSend = "  +CanSend(source));
				return false;
			}
			return true;
		}

		public bool IsValid(Move move)
		{
			return move.TurnsBefore > 0 || IsValid(move.SourceID, move.DestinationID, move.NumShips);
		}

		public void IssueOrder(Move move)
		{
			if (!IsValid(move))
			{
			#if LOG
				Logger.Log("  !Invalid move: from " + move.SourceID + " to " + move.DestinationID + " num " + move.NumShips);
			#endif
				return;
			}

			if (move.TurnsBefore == 0)
			{
				Console.WriteLine("" + move.SourceID + " " + move.DestinationID + " " + move.NumShips);
				Console.Out.Flush();
			}
#if LOG
			//Logger.Log("" + move.SourceID + " " + move.DestinationID +" " + move.NumSheeps);
#endif

			MakeMove(move);
		}

		private void MakeMove(Move move)
		{
			if (move.TurnsBefore == 0)
			{
				fleets.Add(MoveToFleet(1, move));
				//Logger.Log("planet:" + GetPlanet(move.SourceID) + " cansend: " + CanSend(GetPlanet(move.SourceID)) + " safecansend: " + CanSendSafe(GetPlanet(move.SourceID)) + " move: " +move);
				GetPlanet(move.SourceID).RemoveShips(move.NumShips);
				GetPlanetHolder(move.SourceID).ResetFutureStates();
				//Logger.Log("planet after:" + GetPlanet(move.SourceID) + " cansend after: " + CanSend(GetPlanet(move.SourceID)));
			}
			else
			{
				int min = move.NumShips;
				for (int i = 0; i < move.TurnsBefore; i++)
				{
					Planet futurePlanet = PlanetFutureStatus(GetPlanet(move.SourceID), i + 1);
					if (futurePlanet.Owner() != 1)
					{
						min = 0;
						break;
					}
					if (min > futurePlanet.NumShips()) min = futurePlanet.NumShips();
				}

				int canSend = min - move.NumShips;
				GetPlanet(move.SourceID).RemoveShips(canSend < 0 ? move.NumShips : canSend);
			}
			GetPlanetHolder(move.SourceID).ResetFutureStates();
		}

		// Sends the game engine a message to let it know that we're done sending
		// orders. This signifies the end of our turn.
		public void FinishTurn()
		{
			Console.WriteLine("go");
			Console.Out.Flush();
#if LOG
			Logger.Log("go");
#endif
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
						/*fleets.Add(new Fleet(owner,
												numShips,
												source,
												destination,
												totalTripLength,
												turnsRemaining));*/

						bool found = false;
						for (int index = 0; index < fleets.Count; index++)
						{
							Fleet fleet = fleets[index];
							if (fleet.DestinationPlanet() != destination || fleet.TurnsRemaining() != turnsRemaining ||
							    fleet.Owner() != owner) continue;
							fleet.AddShips(numShips);
							found = true;
							break;
						}
						if (!found)
						{
							fleets.Add(new Fleet(owner,
							                    numShips,
							                    source,
							                    destination,
							                    totalTripLength,
							                    turnsRemaining));
						}
					}
				}
			}
		}

		// Store all the planets and fleets. OMG we wouldn't wanna lose all the
		// planets and fleets, would we!?
		private readonly Planets planets;
		private readonly Fleets fleets;
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

			Planets selectedPlanets = new Planets(Config.MaxPlanets);

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
			Planets attackedPlanets = new Planets(Config.MaxPlanets);

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
			Planets myInvasionNeutralPlanetsUnderAttack = new Planets(Config.MaxPlanets);
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
			Planets nearbyPlanets = new Planets(Config.MaxPlanets);

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
			Planets endangeredPlanets = new Planets(Config.MaxPlanets);

			foreach (PlanetHolder planetHolder in myPlanetHolders)
			{
				if (planetHolder.GetOwnerSwitchesToEnemy().Count > 0)
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

		private static int GetFleetsShipNumWithCondition(IEnumerable<Fleet> fleetList, int treshold, int sign)
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

		private static int GetLimitFleetDistance(IEnumerable<Fleet> fleetList, int limitType)
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
		private int myFutureProduction = -1;
		private int enemyFutureProduction = -1;

		public int MyProduction
		{
			get
			{
				if (myProduction == -1) CalcProduction();
				return myProduction;
			}
		}

		public int EnemyProduction
		{
			get
			{
				if (enemyProduction == -1) CalcProduction();
				return enemyProduction;
			}
		}

		public int MyFutureProduction
		{
			get
			{
				if (myFutureProduction == -1) CalcFutureProduction();
				return myFutureProduction;
			}
		}

		public int EnemyFutureProduction
		{
			get
			{
				if (enemyFutureProduction == -1) CalcFutureProduction();
				return enemyFutureProduction;
			}
		}

		private void CalcProduction()
		{
			myProduction = 0;
			enemyProduction = 0;
			Planets allPlanets = Planets();
			foreach (Planet planet in allPlanets)
			{
				if (planet.Owner() == 1) myProduction += planet.GrowthRate();
				if (planet.Owner() == 2) enemyProduction += planet.GrowthRate();
			}
		}

		private void CalcFutureProduction()
		{
			myFutureProduction = 0;
			enemyFutureProduction = 0;
			PlanetHolders allPlanetHolders = PlanetHolders();
			foreach (PlanetHolder planetHolder in allPlanetHolders)
			{
				int lastOwner = GetLastOwner(planetHolder);
				if (lastOwner == 1) myFutureProduction += planetHolder.GetPlanet().GrowthRate();
				if (lastOwner == 2) enemyFutureProduction += planetHolder.GetPlanet().GrowthRate();
			}
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

		public int FutureProduction(int playerID)
		{
			switch (playerID)
			{
				case 1:
					return MyFutureProduction;
				case 2:
					return EnemyFutureProduction;
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
				if (myTotalShipCount == -1) CalcTotalShipCount();
				return myTotalShipCount;
			}
		}

		public int EnemyTotalShipCount
		{
			get
			{
				if (enemyTotalShipCount == -1) CalcTotalShipCount();
				return enemyTotalShipCount;
			}
		}

		private void CalcTotalShipCount()
		{
			myTotalShipCount = 0;
			enemyTotalShipCount = 0;
			foreach (Planet planet in planets)
			{
				if (planet.Owner() == 1) myTotalShipCount += planet.NumShips();
				if (planet.Owner() == 2) enemyTotalShipCount += planet.NumShips();
			}
			foreach (Fleet fleet in fleets)
			{
				if (fleet.Owner() == 1) myTotalShipCount += fleet.NumShips();
				if (fleet.Owner() == 2) enemyTotalShipCount += fleet.NumShips();
			}

			/*foreach (Planet planet in planets)
			{
				Planet futurePlanet = PlanetFutureStatus(planet, Config.MaxTurns - Config.CurrentTurn);
				if (futurePlanet.Owner() == 1) myTotalShipCount += futurePlanet.NumShips();
				if (futurePlanet.Owner() == 2) enemyTotalShipCount += futurePlanet.NumShips();
			}
			foreach (Fleet fleet in fleets.Where(fleet => fleet.TurnsRemaining() + Config.CurrentTurn > Config.MaxTurns))
			{
				if (fleet.Owner() == 1) myTotalShipCount += fleet.NumShips();
				if (fleet.Owner() == 2) enemyTotalShipCount += fleet.NumShips();
			}*/
		}

		public int TotalShipCount(int playerID)
		{
			switch (playerID)
			{
				case 1:
					return MyTotalShipCount;
				case 2:
					return EnemyTotalShipCount;
				default :
					return 0;
			}
		}

		public List<Step> GetMyPlanetSaveSteps(Planet planet)
		{
			List<Step> saveSteps = new List<Step>();

			PlanetHolder planetHolder = GetPlanetHolder(planet);

			List<PlanetOwnerSwitch> switches = planetHolder.GetOwnerSwitchesFromMyToEnemy();
			if (switches.Count == 0) return saveSteps;

			for (int i = 0; i < switches.Count; i++)
			{
				Step step = null;
				int turn = switches[i].TurnsBefore;

				Planet planetInFuture = planetHolder.GetFutureState(turn);
				if (planetInFuture.Owner() != 1)
				{
					step = new Step(0, turn, planetInFuture.NumShips() + Config.MinShipsOnPlanetsAfterDefend);
				}
				else if (planetInFuture.NumShips() < Config.MinShipsOnPlanetsAfterDefend)
				{
					step = new Step(0, turn, Config.MinShipsOnPlanetsAfterDefend - planetInFuture.NumShips());
				}

				if (step != null)
				{
					saveSteps.Add(step);
				}

			}
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

		public int CanSendSafe(Planet planet)
		{
			Planet closestEnemyPlanet = GetClosestPlanet(planet, EnemyPlanets());
			if (closestEnemyPlanet == null)
			{
				return planet.NumShips();
			}
			int distance = Distance(planet, closestEnemyPlanet);
			Planets closePlanets = EnemyPlanetsWithinProximityToPlanet(planet, distance);

			int ships = 0;
			foreach (Planet closePlanet in closePlanets)
			{
				ships += closePlanet.NumShips();
			}

			int safeCanSend = Math.Max(0, (planet.NumShips() - (ships - planet.GrowthRate() * distance)));
			//Logger.Log("Safe: " + safeCanSend + "  notSafe:" + CanSend(planet));
			//if (MyPlanets().Count == 1) return safeCanSend;
			//if (distance > 6) return CanSend(planet);
				//GetEnemyAid(planet, safeTurns);));
			return Math.Min(safeCanSend, CanSend(planet));
		}

		public int CanSendByPlanets(Planet source, Planet dest)
		{
			if (dest.GrowthRate() < source.GrowthRate()) return CanSendSafe(source);
			return CanSend(source);
			
		}

		public int CanSendByPlanets(Planet source, Planet dest, int turns)
		{
			if (dest.GrowthRate() > source.GrowthRate())
			{
				if (turns == 0) return CanSend(source);
				return CanSend(source, turns);
			}
			return CanSendSafe(source);
		}

		//# Returns a string representation of the entire game state.
		public static string SerializeGameState(List<Planet> planets, List<Fleet> fleets, bool forLog)
		{
			string message = "";
			int n = 0;
			foreach (Planet p in planets)
			{
				message += 
					(forLog ? "\"" : "") + 
					SerializePlanet(p) + 
					"#" + 
					n++ + 
					(forLog ? "\\n\" +" : "") +
					"\n";
			}
			message += "\n";

			message = fleets.Aggregate(message, (current, f) => current + ((forLog ? "\"" : "") + SerializeFleet(f) + (forLog ? "\\n\" +" : "") + "\n"));

			message +=
				//"\n" + 
				(forLog ? "\"" : "") +
				"go" +
				(forLog ? "\\n\"" : "")+
				"\n";
			message = message.Replace("\n\n", "\n");
			return message;
		}

		public static string SerializeGameState(PlanetWars pw, bool forLog)
		{
			return SerializeGameState(pw.Planets(), pw.Fleets(), forLog);
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

		public Planet GetClosestPlanet(Planet planet, Planets planetList)
		{
			int distance = int.MaxValue;
			Planet closestPlanet = null;
			foreach (Planet eachPlanet in planetList)
			{
				int currentDistance = Distance(planet, eachPlanet);
				if (distance > currentDistance)
				{
					distance = currentDistance;
					closestPlanet = eachPlanet;
				}
			}
			return closestPlanet;
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

		public Fleet MoveToFleet(int owner, Move move)
		{
			int distance = Distance(move.SourceID, move.DestinationID) + move.TurnsBefore;
			Fleet fleet = new Fleet(
				owner,
				move.NumShips,
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

			if (numberOfTurns > Router.MaxDistance) 
				return GetEnemyAid(planet, Router.MaxDistance);
			if (enemyAid[planet.PlanetID(), numberOfTurns] != -1) 
				return enemyAid[planet.PlanetID(), numberOfTurns];

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

				//enemyCanSend
				int turnsCount = GetPlanetHolder(enemyPlanet.PlanetID()).TurnsCount;
				for (int turn = sendTurn + 1; turn < turnsCount; turn++)
				{
					futurePlanet = PlanetFutureStatus(enemyPlanet, turn);
					if (futurePlanet.Owner() < 2)
					{
						numShips = 0;
						break;
					}
					if (futurePlanet.NumShips() < numShips) numShips = futurePlanet.NumShips();
				}

				enemyAid[planet.PlanetID(), numberOfTurns] += numShips;
			}
			return enemyAid[planet.PlanetID(), numberOfTurns];
		}

		public void IssueOrder(MovesSet movesSet)
		{
			Moves moves = movesSet.GetMoves();
			foreach (Move move in moves)
			{
#if LOG
				Logger.Log(movesSet.AdviserName + ": " + move);
#endif
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

		public int MaxMovesDistance(Moves moves)
		{
			if (moves.Count == 0) return 0;

			int maxDistance = 0;
			foreach (Move move in moves)
			{
				int distance = Distance(move.SourceID, move.DestinationID);
				if (maxDistance < distance) maxDistance = distance;
			}
			return maxDistance;
		}

		public Sectors GetSector(int basePlanetID, int objectPlanetID)
		{
			return Router.GetSector(basePlanetID, objectPlanetID);
		}

		public Sectors GetSector(Planet basePlanet, Planet objectPlanet)
		{
			return Router.GetSector(basePlanet.PlanetID(), objectPlanet.PlanetID());
		}

		public Planets GetClosestPlanetsToTargetBySectors(Planet target, Planets planetList)
		{
			Planets closestPlanets = new Planets(Config.MaxPlanets);
			if (planetList.Count == 0) return closestPlanets;

			//Pair: Sector value, planetID
			List<Pair<Sectors, int>> pairs = new List<Pair<Sectors, int>>();
			foreach (Sectors value in Enum.GetValues(typeof(Sectors)))
			{
				if (value == Sectors.None) continue;

				pairs.Add(new Pair<Sectors, int>(value, -1));
			}

			foreach (Planet planet in planetList)
			{
				if (planet == target) continue;
				Sectors sector = GetSector(target, planet);

				foreach (Pair<Sectors, int> pair in pairs)
				{
					if (pair.First == sector)
					{
						if (pair.Second == -1) pair.Second = planet.PlanetID();
						else if (Distance(planet, target) < Distance(pair.Second, target.PlanetID()))
							pair.Second = planet.PlanetID();
						break;
					}
				}
			}

			foreach (Pair<Sectors, int> pair in pairs)
			{
				if (pair.Second != -1)
					closestPlanets.Add(GetPlanet(pair.Second));
			}
			return closestPlanets;
		}

		public Planets GetSectorPlanetsFromTarget(Planet target, Sectors sector, Planets planetList)
		{
			Planets sectorPlanets = new Planets(Config.MaxPlanets);
			if (planetList.Count == 0) return sectorPlanets;

			foreach (Planet planet in planetList)
			{
				if (planet == target) continue;
				if (sector == GetSector(target, planet))
				{
					sectorPlanets.Add(planet);
				}
			}
			return sectorPlanets;
		}

		private Planets additionalTargetPlanets;
		public void AddTargetPlanet(Planet targetPlanet)
		{
			if (additionalTargetPlanets == null)
				additionalTargetPlanets = new Planets(Config.MaxPlanets);
			if (targetPlanet != null) 
				additionalTargetPlanets.Add(targetPlanet);
		}

		public int GetLastOwner(PlanetHolder planetHolder)
		{
			int lastOwner = planetHolder.GetPlanet().Owner();
			if (planetHolder.OwnerSwitches.Count > 0)
			{
				lastOwner = planetHolder.OwnerSwitches[planetHolder.OwnerSwitches.Count - 1].NewOwner;
			}
			return lastOwner;
		}

		public Planets GetPlanetsByLastOwner(IEnumerable<PlanetHolder> planetHoldersList, int owner)
		{
			Planets targetPlanets = new Planets();
			foreach (PlanetHolder planetHolder in planetHoldersList)
			{
				int lastOwner = GetLastOwner(planetHolder);
				if (lastOwner == owner) targetPlanets.Add(planetHolder.GetPlanet());
			}
			return targetPlanets;
		}

		private Planets frontPlanets;
		public Planets GetFrontPlanets()
		{
			if (frontPlanets == null)
			{
				frontPlanets = new Planets(Config.MaxPlanets);

				Planets targetPlanets = GetPlanetsByLastOwner(PlanetHolders(), 2);
				if (additionalTargetPlanets != null) targetPlanets.AddRange(additionalTargetPlanets);

				Planets myPlanets = GetPlanetsByLastOwner(PlanetHolders(), 1);
				foreach (Planet targetPlanet in targetPlanets)
				{
					Planets closestPlanets = new Planets(Config.MaxPlanets);

					if (Config.UseSectorsForFront) closestPlanets = GetClosestPlanetsToTargetBySectors(targetPlanet, myPlanets);
					else
					{
						Planet closestPlanet = GetClosestPlanet(targetPlanet, myPlanets);
						if (closestPlanet != null) closestPlanets.Add(closestPlanet);
					}

					Comparer comparer = new Comparer(this) {TargetPlanet = targetPlanet};

					closestPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

					for (int i = 0; i < closestPlanets.Count; i++)
					{
						if (frontPlanets.IndexOf(closestPlanets[i]) != -1) continue;

						bool isCloseEnough = true;
						for (int j = 0; j < i; j++)
						{
							if (Distance(closestPlanets[i], closestPlanets[j]) <
								Distance(closestPlanets[i], targetPlanet))
							{
								isCloseEnough = false;
								break;
							}
						}
						if (isCloseEnough) frontPlanets.Add(closestPlanets[i]);

					}
				}
			}
			return frontPlanets;
		}

		public Fleets GetFleetsCloserThan(Fleets fleetList, int treshold)
		{
			Fleets closerFleets = new Fleets();
			foreach (Fleet fleet in fleetList)
			{
				if (fleet.TurnsRemaining() <= treshold) closerFleets.Add(fleet);
			}
			return closerFleets;
		}
	}
}
