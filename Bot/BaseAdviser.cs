using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public abstract class BaseAdviser : IAdviser
	{
		protected BaseAdviser(PlanetWars context)
		{
			Context = context;
		}

		private PlanetWars context;
		public PlanetWars Context
		{
			get { return context; }
			set { context = value; }
		}

		public abstract Moves Run(Planet planet);
		public abstract string GetAdviserName();

		public abstract List<MovesSet> RunAll();
	}
}
