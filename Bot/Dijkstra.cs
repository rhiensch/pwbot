using System;
using System.Collections.Generic;

namespace Bot
{
	class Dijkstra
	{
		/* Takes adjacency matrix in the following format, for a directed graph (2-D array)
		 * Ex. node 1 to 3 is accessible at a cost of 4
		 *        0  1  2  3  4 
		 *   0  { 0, 2, 5, 0, 0},
		 *   1  { 0, 0, 0, 4, 0},
		 *   2  { 0, 6, 0, 0, 8},
		 *   3  { 0, 0, 0, 0, 9},
		 *   4  { 0, 0, 0, 0, 0}
		 */

		/* Resulting arrays with distances to nodes and how to get there */
		private int[] dist;
		public int[] Dist
		{
			get { return dist; }
			private set { dist = value; }
		}

		private int[] path;
		public int[] Path
		{
			get { return path; }
			private set { path = value; }
		}

		/* Holds queue for the nodes to be evaluated */
		private readonly List<int> queue = new List<int>();

		/* Sets up initial settings */
		private void Initialize(int start, int len)
		{
			Dist = new int[len];
			Path = new int[len];

			/* Set distance to all nodes to infinity - alternatively use Int.MaxValue for use of Int type instead */
			for (int i = 0; i < len; i++)
			{
				Dist[i] = int.MaxValue;

				queue.Add(i);
			}

			/* Set distance to 0 for starting point and the previous node to null (-1) */
			Dist[start] = 0;
			Path[start] = -1;
		}

		/* Retrives next node to evaluate from the queue */
		private int GetNextVertex()
		{
			double min = Double.PositiveInfinity;
			int vertex = -1;

			/* Search through queue to find the next node having the smallest distance */
			foreach (int j in queue)
			{
				if (Dist[j] <= min)
				{
					min = Dist[j];
					vertex = j;
				}
			}

			queue.Remove(vertex);

			return vertex;
		}

		/* Takes a graph as input an adjacency matrix (see top for details) and a starting node */
		public Dijkstra(int[,] graph, int start)
		{
			/* Check graph format and that the graph actually contains something */
			if (graph.GetLength(0) < 1 || graph.GetLength(0) != graph.GetLength(1))
			{
				throw new ArgumentException("Graph error, wrong format or no nodes to compute");
			}

			int len = graph.GetLength(0);

			Initialize(start, len);

			while (queue.Count > 0)
			{
				int u = GetNextVertex();

				/* Find the nodes that u connects to and perform relax */
				for (int v = 0; v < len; v++)
				{
					/* Checks for edges with negative weight */
					if (graph[u, v] < 0)
					{
						throw new ArgumentException("Graph contains negative edge(s)");
					}

					/* Check for an edge between u and v */
					if (graph[u, v] > 0)
					{
						/* Edge exists, relax the edge */
						if (Dist[v] > Dist[u] + graph[u, v])
						{
							Dist[v] = Dist[u] + graph[u, v];
							Path[v] = u;
						}
					}
				}
			}
		}
	}
}
