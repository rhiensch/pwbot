using System;
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

		public int CompareGrowsRateGT(Planet planet1, Planet planet2)
		{
			return (planet2.GrowthRate() - planet1.GrowthRate());
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
				(Context.GetClosestMyPlanetDistance(planet1) -
				 Context.GetClosestMyPlanetDistance(planet2))
				//(Context.GetPlanetSummaryDistance(myPlanets, planet1) -
				// Context.GetPlanetSummaryDistance(myPlanets, planet2))
				* Config.DistanceKoef;

			return growthDifference + numFleetsDifference + distanceDifference;
		}

		public int CompareTurnsRemainingLT(Fleet fleet1, Fleet fleet2)
		{
			return (fleet1.TurnsRemaining() - fleet2.TurnsRemaining());
		}

		private Planet targetPlanet;
		public Planet TargetPlanet
		{
			get { return targetPlanet; }
			set { targetPlanet = value; }
		}

		public int CompareDistanceToTargetPlanetLT(Planet planet1, Planet planet2)
		{
			if (TargetPlanet == null) throw new ArgumentNullException("planet1", "Target planet is not defined!");
			if (planet1.PlanetID() == planet2.PlanetID()) return 0;

			return (Context.Distance(planet1, TargetPlanet) - Context.Distance(planet2, TargetPlanet));
		}

		public int CompareSetScoreGT(MovesSet set1, MovesSet set2)
		{
			return (Math.Sign(set2.Score - set1.Score));
		}

		public int Coordinates(Planet planet1, Planet planet2)
		{
			/*int result = planet1.X() > planet2.X() ? 1 : -1;
			if (planet1.X() == planet2.X())
				result = planet1.Y() > planet2.Y() ? 1 : -1;
			return result;*/
			return 0;
		}
	}
}
