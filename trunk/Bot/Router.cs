#undef LOG

using System;
using System.Collections.Generic;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public enum Sectors { None = 0, NordEast, SouthEast, SouthWest, NordWest };

	static class Router
	{
		private static int[,] distances;
		private static Sectors[,] sectors;

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

		public static int MaxDistance { get; private set; }

		private static void CalcMaxDistance()
		{
			MaxDistance = 0;
			for (int i = 0; i < planets.Count; i++)
			{
				for (int j = i + 1; j < planets.Count; j++)
				{
					if (Distance(i, j) > MaxDistance) MaxDistance = Distance(i, j);
				}
			}
		}

		public static void Init(Planets planetList)
		{
			double newControlSum = GetControlSum(planetList);
			if (newControlSum == controlSum) return;

			controlSum = newControlSum;
			planets = new Planets(planetList);
			distances = new int[planets.Count, planets.Count];
			sectors = new Sectors[planets.Count, planets.Count];

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

		/*		  |
		 *		  |		object
		 *		  |
		 *------base-----------
		 *		  |
		 * return: NordEast
		 */
		public static Sectors GetSector(int basePlanetID, int objectPlanetID)
		{
			if (distances == null)
				throw new NullReferenceException("Router was not initialized!");
			if (sectors[basePlanetID, objectPlanetID] == Sectors.None)
				CalcSector(basePlanetID, objectPlanetID);

			return sectors[basePlanetID, objectPlanetID];
		}

		public static Sectors GetSector(Planet basePlanet, Planet objectPlanet)
		{
			return GetSector(basePlanet.PlanetID(), objectPlanet.PlanetID());
		}

		private static void CalcSector(int basePlanetID, int objectPlanetID)
		{
			double baseX = planets[basePlanetID].X();
			double baseY = planets[basePlanetID].Y();
			double objectX = planets[objectPlanetID].X();
			double objectY = planets[objectPlanetID].Y();

			Sectors result;
			if (objectY >= baseY)
				result = objectX >= baseX ? Sectors.NordEast : Sectors.NordWest;
			else
				result = objectX >= baseX ? Sectors.SouthEast : Sectors.SouthWest;

			sectors[basePlanetID, objectPlanetID] = result;
			sectors[objectPlanetID, basePlanetID] = MirrorSector(result);
		}

		public static Sectors MirrorSector(Sectors sector)
		{
			switch (sector)
			{
				case Sectors.NordEast:
					return Sectors.SouthWest;
				case Sectors.NordWest:
					return Sectors.SouthEast;
				case Sectors.SouthEast:
					return Sectors.NordWest;
				case Sectors.SouthWest:
					return Sectors.NordEast;
				default:
					return Sectors.None;
			}
		}
	}
}
