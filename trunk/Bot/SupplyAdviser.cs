using System.Collections.Generic;
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

			int canSend = Context.CanSend(supplyPlanet);
			if (canSend == 0) return moves;

			IPathFinder pathFinder = new DijkstraPathFinder(Context);
			Planet dest = pathFinder.FindNextPlanetInPath(supplyPlanet);
			if (dest != null)
			{
				moves.Add(new Move(supplyPlanet.PlanetID(), dest.PlanetID(), canSend));
			}
			return moves;

			/*int supplyPlanetFrontLevel = Context.GetClosestEnemyPlanetDistance(supplyPlanet);
				//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), supplyPlanet);

			Planets nearPlanets = Context.MyPlanets();
			if (nearPlanets.Count == 0) return moves;

			Comparer comparer = new Comparer(Context);
			comparer.TargetPlanet = supplyPlanet;
			nearPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			foreach (Planet nearPlanet in nearPlanets)
			{
				if (nearPlanet == supplyPlanet) continue;

				int nearPlanetFrontLevel = Context.GetClosestEnemyPlanetDistance(nearPlanet);
					//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), nearPlanet);

				if (nearPlanetFrontLevel < supplyPlanetFrontLevel)
				{
					moves.Add(new Move(supplyPlanet.PlanetID(), nearPlanet.PlanetID(), canSend));
					return moves;
				}
			}
			return moves;*/
		}

		public override string GetAdviserName()
		{
			return "Supply";
		}

		public override List<MovesSet> RunAll()
		{
			Planets myPlanets = Context.MyPlanets();

			List<MovesSet> movesSet = new List<MovesSet>();
			//if (Context.GetFrontPlanets().Count == 0) return movesSet;

			foreach (Planet myPlanet in myPlanets)
			{
				Moves moves = Run(myPlanet);
				if (moves.Count > 0)
				{
					MovesSet set = new MovesSet(moves, 0, GetAdviserName(), Context);
					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
