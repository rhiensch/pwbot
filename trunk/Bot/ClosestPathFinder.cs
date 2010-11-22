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

			Planets nearPlanets = Context.Planets();
			if (nearPlanets.Count == 0) return null;

			Comparer comparer = new Comparer(Context) {TargetPlanet = source};
			nearPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			return (from nearPlanet in nearPlanets
			        where nearPlanet.PlanetID() != source.PlanetID()
			        let distance = Context.Distance(nearPlanet, source)
			        let futurePlanet = Context.PlanetFutureStatus(nearPlanet, distance)
			        where futurePlanet.Owner() == 1
			        let nearPlanetFrontLevel = Context.GetClosestPlanetDistance(nearPlanet, enemyPlanets)
			        where nearPlanetFrontLevel < supplyPlanetFrontLevel
			        select nearPlanet).FirstOrDefault();
		}
	}
}
