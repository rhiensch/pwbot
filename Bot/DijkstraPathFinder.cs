using System.Collections.Generic;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class DijkstraPathFinder : IPathFinder
	{
		public PlanetWars Context;
		public DijkstraPathFinder(PlanetWars context)
		{
			Context = context;
		}

		public int[,] CreateGraph()
		{
			int count = Context.Planets().Count;
			int[,] createGraph = new int[count, count];
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < count; j++)
				{
					createGraph[i, j] = 0;
				}
			}

			Planets myPlanets = Context.MyPlanets();
			foreach (Planet planet in myPlanets)
			{
				Planets closestPlanets = Context.GetClosestPlanetsToTargetBySectors(planet, myPlanets);
				foreach (Planet closestPlanet in closestPlanets)
				{
					if (createGraph[planet.PlanetID(), closestPlanet.PlanetID()] != 0) continue;
					int distance = Context.Distance(planet, closestPlanet);
					createGraph[planet.PlanetID(), closestPlanet.PlanetID()] = distance;
					createGraph[closestPlanet.PlanetID(), planet.PlanetID()] = distance;
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
				if (min <= dijkstra.Dist[frontPlanet.PlanetID()] || dijkstra.Dist[frontPlanet.PlanetID()] <= 0) continue;
				min = dijkstra.Dist[frontPlanet.PlanetID()];
				minID = frontPlanet.PlanetID();
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

			return Context.GetPlanet(path[0]);
		}
	}
}
