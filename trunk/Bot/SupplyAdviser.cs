using System.Collections.Generic;
using System.Linq;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class SupplyAdviser : BaseAdviser
	{
		public SupplyAdviser(PlanetWars context)
			: base(context)
		{
		}

		public override Moves Run(Planet supplyPlanet)
		{
			Moves moves = new Moves();

			if (supplyPlanet == null) return moves;

			Planets frontPlanets = Context.GetFrontPlanets();
			if (frontPlanets == null) return moves;
			if (frontPlanets.Count == 0) return moves;
			if (frontPlanets.IndexOf(supplyPlanet) != -1) return moves;

			IPathFinder pathFinder = new ClosestPathFinder(Context);
				//new DirectPathFinder(Context);
				//new DijkstraPathFinder(Context);
			Planet dest = pathFinder.FindNextPlanetInPath(supplyPlanet);
			if (dest != null)
			{
				int canSend = Context.CanSend(supplyPlanet);
				if (canSend == 0) return moves;

				Move move = new Move(supplyPlanet, dest, canSend);
				moves.Add(move);
			}
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Supply";
		}

		public override List<MovesSet> RunAll()
		{
			Planets myPlanets = Context.MyPlanets();

			//if (Context.GetFrontPlanets().Count == 0) return movesSet;
			return (from myPlanet in myPlanets
			        select Run(myPlanet)
			        into moves where moves.Count > 0 select new MovesSet(moves, 0, GetAdviserName(), Context)).ToList();
		}
	}
}
