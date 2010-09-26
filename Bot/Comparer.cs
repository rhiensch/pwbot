using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class Comparer
	{
		private PlanetWars context;
		public PlanetWars Context
		{
			get { return context; }
			private set { context = value; }
		}

		public Comparer(PlanetWars context)
		{
			Context = context;
		}

		public int CompareNumberOfShipsLT(Planet planet1, Planet planet2)
		{
			return (planet1.NumShips() - planet2.NumShips());
		}

		public int CompareNumberOfShipsGT(Planet planet1, Planet planet2)
		{
			return (planet2.NumShips() - planet1.NumShips());
		}

		public int CompareSecondOfPair(Pair<int, int> pair1, Pair<int, int> pair2)
		{
			return pair2.Second - pair1.Second;
		}

		public int CompareImportanceOfPlanetsGT(Planet planet1, Planet planet2)
		{
			if (planet1.PlanetID() == planet2.PlanetID()) return 0;

			Planets myPlanets = Context.MyPlanets();

			int growthDifference =
				(planet1.GrowthRate() -
				 planet2.GrowthRate())
				* Config.GrowsRateKoef;
			int distanceDifference =
				(Context.GetPlanetSummaryDistance(myPlanets, planet1) -
				 Context.GetPlanetSummaryDistance(myPlanets, planet2))
				* Config.DistanceKoef;

			return growthDifference + distanceDifference;
		}

		public int CompareImportanceOfNeutralPlanetsGT(Planet planet1, Planet planet2)
		{
			if (planet1.PlanetID() == planet2.PlanetID()) return 0;

			Planets myPlanets = Context.MyPlanets();

			int growthDifference =
				(planet1.GrowthRate() -
				 planet2.GrowthRate())
				* Config.GrowsRateKoef;
			int distanceDifference =
				(Context.GetPlanetSummaryDistance(myPlanets, planet1) -
				 Context.GetPlanetSummaryDistance(myPlanets, planet2))
				* Config.DistanceKoef;
			int numFleetsDifference =
				(planet1.NumShips() -
				 planet2.NumShips())
				* Config.NumShipsKoef;

			return growthDifference + distanceDifference + numFleetsDifference;
		}

		public int CompareImportanceOfEnemyPlanetsGT(Planet planet1, Planet planet2)
		{
			if (planet1.PlanetID() == planet2.PlanetID()) return 0;

			Planets myPlanets = Context.MyPlanets();

			int growthDifference =
				(planet1.GrowthRate() -
				 planet2.GrowthRate())
				* Config.GrowsRateKoef;
			int numFleetsDifference=
				(planet1.NumShips() -
				 planet2.NumShips())
				* Config.NumShipsKoef;
			int distanceDifference =
				(Context.GetPlanetSummaryDistance(myPlanets, planet1) -
				 Context.GetPlanetSummaryDistance(myPlanets, planet2))
				* Config.DistanceKoef;

			return growthDifference + numFleetsDifference + distanceDifference;
		}

	}
}
