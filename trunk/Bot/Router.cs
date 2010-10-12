#undef DEBUG

using System;
using System.Collections.Generic;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	static class Router
	{
		private static int[,] distances;
		private static Planets planets;

		private static double controlSum;
		private static double GetControlSum(IEnumerable<Planet> planetList)
		{
			double sum = 0;
			foreach (Planet planet in planetList)
			{
				sum += planet.X() * (planet.PlanetID() + 1);
				sum += planet.Y() * (planet.PlanetID() + 1);
			}
			return sum;
		}

		private static int maxDistance;
		public static int MaxDistance
		{
			get
			{
				return maxDistance;
			}
		}

		private static void CalcMaxDistance()
		{
			maxDistance = 0;
			for (int i = 0; i < planets.Count; i++)
			{
				for (int j = i + 1; j < planets.Count; j++)
				{
					if (Distance(i, j) > maxDistance) maxDistance = Distance(i, j);
				}
			}
		}

		public static void Init(Planets planetList)
		{
			double newControlSum = GetControlSum(planetList);
			if (newControlSum == controlSum) return;

#if DEBUG
			Logger.Log("Router reinitialized!");
#endif
			controlSum = newControlSum;
			planets = new Planets(planetList);
			distances = new int[planets.Count, planets.Count];

			CalcMaxDistance();
		}

		private static int CalcDistance(Planet source, Planet destination)
		{
			double dx = source.X() - destination.X();
			double dy = source.Y() - destination.Y();
			double squared = dx * dx + dy * dy;
			double rooted = Math.Sqrt(squared);
			int result = (int) Math.Ceiling(rooted);

			distances[source.PlanetID(), destination.PlanetID()] = result;
			distances[destination.PlanetID(), source.PlanetID()] = result;
			return result;
		}

		private static int CalcDistance(int planetID1, int planetID2)
		{
			Planet source = planets[planetID1];
			Planet destination = planets[planetID2];

			return CalcDistance(source, destination);
		}

		public static int Distance(int planetID1, int planetID2)
		{
			if (distances == null)
				throw new NullReferenceException("Router was not initialized!");
			if (planetID1 == planetID2) return 0;

			if (distances[planetID1, planetID2] == 0) return CalcDistance(planetID1, planetID2);
			return distances[planetID1, planetID2];
		}

		public static int Distance(Planet planet1, Planet planet2)
		{
			return Distance(planet1.PlanetID(), planet2.PlanetID());
		}
	}
}
