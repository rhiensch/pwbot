using System;
using System.Collections.Generic;
using System.Text;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class PlanetHolder
	{
		private readonly Planet thisPlanet;
		private readonly Fleets thisPlanetFleets;

		private int canSend;
		public int CanSend
		{
			get { return canSend; }
			private set { canSend = value; }
		}

		private Planets futureStates;
		public Planets FutureStates
		{
			get
			{
				if (futureStates == null)
				{
					futureStates = new Planets(Router.MaxDistance + 1);
					CalcFutureState();
				}
				return futureStates;
			}
		}

		public PlanetHolder(Planet planet, Fleets fleetList)
		{
			thisPlanet = planet;
			thisPlanetFleets = fleetList;
		}

		private void CalcFutureState()
		{
			futureStates[0] = thisPlanet;

			Planet planetInFuture = new Planet(thisPlanet);
			for (int turn = 1; turn < Router.MaxDistance + 1; turn++)
			{
				PlanetGrowth(planetInFuture);

				Fleets thisTurnFleets = GetThisTurnFleets(turn);

				CalcFleetsOnPlanet(planetInFuture, thisTurnFleets);

				if (planetInFuture.Owner() != 1) canSend = 0;
				if (planetInFuture.NumShips() < canSend) canSend = planetInFuture.NumShips();

				futureStates[turn] = planetInFuture;
			}
		}

		private static void CalcFleetsOnPlanet(Planet planetInFuture, Fleets thisTurnFleets)
		{
			// First is ownerID, second is number of ships
			List<Pair<int, int>> ships = new List<Pair<int, int>>();

			if (thisTurnFleets.Count > 0)
			{
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
		}

		private static void BattleForPlanet(Planet planetInFuture, List<Pair<int, int>> ships)
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

		private Fleets GetThisTurnFleets(int turn)
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

		private static void PlanetGrowth(Planet planetInFuture)
		{
			if (planetInFuture.Owner() != 0)
			{
				planetInFuture.NumShips(planetInFuture.NumShips() + planetInFuture.GrowthRate());
			}
		}
	}
}
