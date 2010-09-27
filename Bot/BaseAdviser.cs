using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public abstract class BaseAdviser : IAdviser
	{
		protected BaseAdviser(PlanetWars context)
		{
			Context = context;
			if (usedPlanets == null) usedPlanets = new Planets();
		}

		private PlanetWars context;
		public PlanetWars Context
		{
			get { return context; }
			set { context = value; }
		}

		public abstract Moves Run();
		public abstract string GetAdviserName();

		protected Planets usedPlanets;
	}
}
