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

		public abstract Moves Run();
		public abstract string GetAdviserName();
	}
}
