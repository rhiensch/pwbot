using System;
using System.Collections.Generic;
using System.Text;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	class ClosestPathFinder : IPathFinder
	{
		public PlanetWars Context;
		public ClosestPathFinder(PlanetWars context)
		{
			Context = context;
		}

		public Planet FindNextPlanetInPath(Planet source)
		{
			int supplyPlanetFrontLevel = Context.GetClosestEnemyPlanetDistance(source);
			//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), supplyPlanet);

			Planets nearPlanets = Context.MyPlanets();
			if (nearPlanets.Count == 0) return null;

			Comparer comparer = new Comparer(Context);
			comparer.TargetPlanet = source;
			nearPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			foreach (Planet nearPlanet in nearPlanets)
			{
				if (nearPlanet == source) continue;

				int nearPlanetFrontLevel = Context.GetClosestEnemyPlanetDistance(nearPlanet);
				//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), nearPlanet);

				if (nearPlanetFrontLevel < supplyPlanetFrontLevel)
				{
					return nearPlanet;
				}
			}
			return null;
		}
	}
}
