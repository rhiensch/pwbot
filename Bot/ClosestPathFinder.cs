using System.Linq;
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

			//Planets nearPlanets = Context.GetFrontPlanets();
			Planets nearPlanets = Context.Planets();
			if (nearPlanets.Count == 0) return null;

			//if (nearPlanets.Contains(source)) return null;

			Comparer comparer = new Comparer(Context) {TargetPlanet = source};
			nearPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			//return nearPlanets[0];

			Planet result = null;
			int resultFrontLevel = supplyPlanetFrontLevel;
			int resultDistance = 0;
			foreach (Planet nearPlanet in nearPlanets)
			{
				if (nearPlanet.PlanetID() == source.PlanetID()) continue;
				
				int distance = Context.Distance(nearPlanet, source);
				Planet futurePlanet = Context.PlanetFutureStatus(nearPlanet, distance);
				if (futurePlanet.Owner() != 1) continue;

				int nearPlanetFrontLevel = Context.GetClosestPlanetDistance(nearPlanet, enemyPlanets);

				if (nearPlanetFrontLevel < resultFrontLevel)
				{
					if ((result == null) || (distance < resultDistance * 1.2))
					{
						result = nearPlanet;
						resultFrontLevel = nearPlanetFrontLevel;
						resultDistance = distance;
					}
				}
			}
			return result;

			/*return (from nearPlanet in nearPlanets
			        where nearPlanet.PlanetID() != source.PlanetID()
			        let distance = Context.Distance(nearPlanet, source)
			        let futurePlanet = Context.PlanetFutureStatus(nearPlanet, distance)
			        where futurePlanet.Owner() == 1
			        let nearPlanetFrontLevel = Context.GetClosestPlanetDistance(nearPlanet, enemyPlanets)
			        where nearPlanetFrontLevel < supplyPlanetFrontLevel
			        select nearPlanet).FirstOrDefault();*/
		}
	}
}
