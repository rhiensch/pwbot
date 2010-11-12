using System;
using System.Collections.Generic;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class Comparer
	{
		public PlanetWars Context { get; private set; }

		public Comparer(PlanetWars context)
		{
			Context = context;
		}

		public int CompareNumberOfShipsLT(Planet planet1, Planet planet2)
		{
			int result = (planet1.NumShips() - planet2.NumShips());
			if (result == 0) result = planet1.PlanetID() - planet2.PlanetID();
			return result;
		}

		public int CompareNumberOfShipsGT(Planet planet1, Planet planet2)
		{
			int result = (planet2.NumShips() - planet1.NumShips());
			if (result == 0) result = planet1.PlanetID() - planet2.PlanetID();
			return result;
		}

		public int CompareGrowsRateGT(Planet planet1, Planet planet2)
		{
			int result = (planet2.GrowthRate() - planet1.GrowthRate());
			if (result == 0) result = planet1.PlanetID() - planet2.PlanetID();
			return result;
		}

		public int CompareGrowsRateLT(Planet planet1, Planet planet2)
		{
			return -CompareGrowsRateGT(planet1, planet2);
		}

		public int CompareTurnsRemainingLT(Fleet fleet1, Fleet fleet2)
		{
			int result = (fleet1.TurnsRemaining() - fleet2.TurnsRemaining());
			if (result == 0) result = fleet1.NumShips() - fleet2.NumShips();
			if (result == 0) result = fleet1.SourcePlanet() - fleet2.SourcePlanet();
			if (result == 0) result = fleet1.DestinationPlanet() - fleet2.DestinationPlanet();
			return result;
		}

		public Planet TargetPlanet { get; set; }

		public int CompareDistanceToTargetPlanetLT(Planet planet1, Planet planet2)
		{
			if (TargetPlanet == null) throw new ArgumentNullException("planet1", "Target planet is not defined!");
			if (planet1.PlanetID() == planet2.PlanetID()) return planet1.PlanetID() - planet2.PlanetID();

			int result = (Context.Distance(planet1, TargetPlanet) - Context.Distance(planet2, TargetPlanet));
			if (result == 0) result = planet2.GrowthRate() - planet1.GrowthRate();
			if (result == 0) result = planet1.PlanetID() - planet2.PlanetID();

			return result;
		}

		public int CompareSetScoreGT(MovesSet set1, MovesSet set2)
		{
			int result = Math.Sign(set2.Score - set1.Score);
			if (result == 0) result = Math.Sign(set1.AverageDistance - set2.AverageDistance);
			if (result == 0) result = set1.MinDistance - set2.MinDistance;
			return result;
		}

		public int CompareSetListScoreGT(List<MovesSet> setList1, List<MovesSet> setList2)
		{
			double score1 = 0.0;
			double score2 = 0.0;
			foreach (MovesSet movesSet in setList1)
			{
				score1 += movesSet.Score;
			}
			foreach (MovesSet movesSet in setList2)
			{
				score2 += movesSet.Score;
			}
			int result = Math.Sign(score2 - score1);
			return result;
		}
	}
}
