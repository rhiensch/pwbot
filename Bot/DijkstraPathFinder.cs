using System.Collections.Generic;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class DijkstraPathFinder
	{
		public PlanetWars Context;
		public DijkstraPathFinder(PlanetWars context)
		{
			Context = context;
		}

		public int[,] CreateGraph()
		{
			int[,] createGraph = new int[Context.Planets().Count, Context.Planets().Count];

			Planets planets = Context.MyPlanets();
			foreach (Planet planet in planets)
			{
				Planets closestPlanets = Context.GetClosestPlanetsToTargetBySectors(planet, planets);
				foreach (Planet closestPlanet in closestPlanets)
				{
					if (createGraph[planet.PlanetID(), closestPlanet.PlanetID()] == 0)
					{
						int distance = Context.Distance(planet, closestPlanet);
						createGraph[planet.PlanetID(), closestPlanet.PlanetID()] = distance;
						createGraph[closestPlanet.PlanetID(), planet.PlanetID()] = distance;

						System.Console.WriteLine(planet.PlanetID() + " -> " + closestPlanet.PlanetID() + " = " + distance);
					}
				}
			}

			return createGraph;
		}

		private int[,] graph;
		public int[,] Graph
		{
			get { return graph ?? (graph = CreateGraph()); }
		}

		public Planet FindNextPlanetInPath(Planet source)
		{
			Dijkstra dijkstra = new Dijkstra(Graph, source.PlanetID());
			int min = int.MaxValue;
			int minID = -1;

			Planets frontPlanets = Context.GetFrontPlanets();
			foreach (Planet frontPlanet in frontPlanets)
			{
				if (min > dijkstra.Dist[frontPlanet.PlanetID()] &&
					dijkstra.Dist[frontPlanet.PlanetID()] > 0)
				{
					min = dijkstra.Dist[frontPlanet.PlanetID()];
					minID = frontPlanet.PlanetID();
				}
			}

			if (minID == -1) return null;

			string s = "";

			List<int> path = new List<int>();
			for (int v = minID; v != source.PlanetID(); v = dijkstra.Path[v])
			{
				path.Add(v);
				s = v + "," +s;
			}
			path.Reverse();

			System.Console.WriteLine(s);

			return Context.GetPlanet(path[0]);
		}
	}
}
