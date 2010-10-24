namespace Bot
{
	public class DirectPathFinder : IPathFinder
	{
		public PlanetWars Context;
		public DirectPathFinder(PlanetWars context)
		{
			Context = context;
		}

		public Planet FindNextPlanetInPath(Planet source)
		{
			Planet dest = Context.GetClosestPlanet(source, Context.GetFrontPlanets());
			return dest;
		}
	}
}
