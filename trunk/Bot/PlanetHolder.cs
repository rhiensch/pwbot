using System.Collections.Generic;
using System.Linq;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class PlanetHolder
	{
		private readonly Planet thisPlanet;
		private readonly Fleets thisPlanetFleets;
		public readonly int TurnsCount;

		public PlanetHolder(Planet planet, Fleets fleetList)
		{
			thisPlanet = planet;
			thisPlanetFleets = fleetList;

			TurnsCount = PlanetWars.GetFarestFleetDistance(thisPlanetFleets);
			ownerSwitches = new List<PlanetOwnerSwitch>();
		}

		public Planet GetPlanet()
		{
			return thisPlanet;
		}

		private readonly List<PlanetOwnerSwitch> ownerSwitches;
		public List<PlanetOwnerSwitch> OwnerSwitches
		{
			get
			{
				FillFutureStatesIfNeeded();
				return ownerSwitches;
			}
		}

		public bool IsNeutralToEnemySwith(int turn)
		{
			List<PlanetOwnerSwitch> switches = GetOwnerSwitchesFromNeutralToEnemy();
			return switches.Any(planetOwnerSwitch => planetOwnerSwitch.TurnsBefore == turn);
		}

		public List<PlanetOwnerSwitch> GetOwnerSwitchesFromNeutralToEnemy()
		{
			return GetOwnerSwitchesToEnemy(0);
		}

		public List<PlanetOwnerSwitch> GetOwnerSwitchesFromMyToEnemy()
		{
			return GetOwnerSwitchesToEnemy(1);
		}

		public List<PlanetOwnerSwitch> GetOwnerSwitchesFromEnemyToMy()
		{
			return OwnerSwitches.Where(planetOwnerSwitch => planetOwnerSwitch.OldOwner > 1).ToList();
		}

		private List<PlanetOwnerSwitch> GetOwnerSwitchesToEnemy(int oldOwner)
		{
			return OwnerSwitches.Where(planetOwnerSwitch => planetOwnerSwitch.OldOwner == oldOwner && planetOwnerSwitch.NewOwner > 1).ToList();
		}

		public List<PlanetOwnerSwitch> GetOwnerSwitchesToEnemy()
		{
			return OwnerSwitches.Where(planetOwnerSwitch => planetOwnerSwitch.NewOwner > 1).ToList();
		}

		private int canSend;
		public int CanSend()
		{
			FillFutureStatesIfNeeded();
			return canSend;
		}

		public int CanSend(int turns)
		{
			if (turns == 0) return CanSend();
			FillFutureStatesIfNeeded();

			int canSendTurns = GetFutureState(turns).NumShips();
			for (int i = turns + 1; i < TurnsCount; i++)
			{
				Planet futurePlanet = GetFutureState(i);
				if (futurePlanet.Owner() != 1) return 0;
				if ((futurePlanet.NumShips() < canSendTurns))
					canSendTurns = futurePlanet.NumShips();
			}
			return canSendTurns;
		}

		private Planets futureStates;

		public Planet GetFutureState(int numberOfTurns)
		{
			FillFutureStatesIfNeeded();
			if (numberOfTurns > TurnsCount)
			{
				Planet futureState = new Planet(futureStates[TurnsCount]);
				if (futureState.Owner() <= 0) return futureState;
				futureState.NumShips(futureState.NumShips() +
				                     futureState.GrowthRate() * (numberOfTurns - TurnsCount));
				return futureState;
			}
			return futureStates[numberOfTurns];
		}

		private void FillFutureStatesIfNeeded()
		{
			if (futureStates != null) return;
			futureStates = new Planets(TurnsCount);
			CalcFutureState();
		}

		public void ResetFutureStates()
		{
			futureStates = null;
		}

		private void CalcFutureState()
		{
			futureStates.Clear();
			futureStates.Add(thisPlanet);

			canSend = thisPlanet.NumShips();

			for (int turn = 1; turn <= TurnsCount; turn++)
			{
				Planet planetInFuture = new Planet(futureStates[turn - 1]);
				PlanetGrowth(planetInFuture);

				Fleets thisTurnFleets = GetThisTurnFleets(turn);

				int oldPlanetOwner = planetInFuture.Owner();

				CalcFleetsOnPlanet(planetInFuture, thisTurnFleets);

				if (planetInFuture.Owner() != oldPlanetOwner)
				{
					
					PlanetOwnerSwitch pos = new PlanetOwnerSwitch(
						oldPlanetOwner, planetInFuture.Owner(), turn);

					ownerSwitches.Add(pos);
				}

				futureStates.Add(planetInFuture);

				if (planetInFuture.Owner() != 1) canSend = 0;
				if (planetInFuture.NumShips() >= canSend) continue;
				canSend = planetInFuture.NumShips();
			}
		}

		private static void CalcFleetsOnPlanet(Planet planetInFuture, Fleets thisTurnFleets)
		{
			// First is ownerID, second is number of ships
			List<Pair<int, int>> ships = new List<Pair<int, int>>();

			if (thisTurnFleets.Count <= 0) return;
			const int owners = 2;

			for (int id = 1; id <= owners; ++id)
			{
				Fleets ownerFleets = PlanetWars.FleetsWithGivenOwner(thisTurnFleets, id);
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

		private static void BattleForPlanet(Planet planetInFuture, List<Pair<int, int>> ships)
		{
			// Were there any fleets other than the one on the planet?
			if (ships.Count <= 1) return;
			// Sorts the fleets in descending order by the number of ships in the fleet
			ships.Sort(Pair<int, int>.CompareSecondOfPair);

			Pair<int, int> winner = ships[0];
			Pair<int, int> secondToWinner = ships[1];

			if (winner.Second == secondToWinner.Second)
			{
				//old owner stays
				planetInFuture.NumShips(0);
			}
			else
			{
				planetInFuture.Owner(winner.First);
				planetInFuture.NumShips(winner.Second - secondToWinner.Second);
			}
		}

		private Fleets GetThisTurnFleets(int turn)
		{
			return thisPlanetFleets.Where(fleet => fleet.TurnsRemaining() == turn).ToList();
		}

		private static void PlanetGrowth(Planet planetInFuture)
		{
			if (planetInFuture.Owner() != 0)
			{
				planetInFuture.NumShips(planetInFuture.NumShips() + planetInFuture.GrowthRate());
			}
		}

		public int Owner()
		{
			return thisPlanet.Owner();
		}
	}
}
