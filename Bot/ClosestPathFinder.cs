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
			Planets enemyPlanets = Context.GetPlanetsByLastOwner(Context.PlanetHolders(), 2);

			int supplyPlanetFrontLevel = Context.GetClosestPlanetDistance(source, enemyPlanets);
			//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), supplyPlanet);

			Planets nearPlanets = Context.Planets();
			if (nearPlanets.Count == 0) return null;

			Comparer comparer = new Comparer(Context) {TargetPlanet = source};
			nearPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			foreach (Planet nearPlanet in nearPlanets)
			{
				if (nearPlanet.PlanetID() == source.PlanetID()) continue;
				int distance = Context.Distance(nearPlanet, source);
				Planet futurePlanet = Context.PlanetFutureStatus(nearPlanet, distance);

				if (futurePlanet.Owner() != 1) continue;
				int nearPlanetFrontLevel = Context.GetClosestPlanetDistance(nearPlanet, enemyPlanets);
					
				//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), nearPlanet););

				if (nearPlanetFrontLevel < supplyPlanetFrontLevel)
				{
					return nearPlanet;
				}
			}
			return null;
		}
	}
}
