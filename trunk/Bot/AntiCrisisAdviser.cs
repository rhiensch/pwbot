using System;
using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class AntiCrisisAdviser : BaseAdviser
	{
		public AntiCrisisAdviser(PlanetWars context)
			: base(context)
		{
		}

		public override Moves Run(Planet supplyPlanet)
		{
			Moves moves = new Moves();

			/*if (supplyPlanet == null) return moves;

			Planets frontPlanets = Context.GetFrontPlanets();
			if (frontPlanets == null) return moves;
			if (frontPlanets.Count == 0) return moves;
			if (frontPlanets.IndexOf(supplyPlanet) != -1) return moves;

			int canSend = Context.CanSend(supplyPlanet);
			if (canSend == 0) return moves;

			IPathFinder pathFinder = new ClosestPathFinder(Context);
			//new DijkstraPathFinder(Context);
			Planet dest = pathFinder.FindNextPlanetInPath(supplyPlanet);
			if (dest != null)
			{
				moves.Add(new Move(supplyPlanet.PlanetID(), dest.PlanetID(), canSend));
			}*/
			return moves;
		}

		public override string GetAdviserName()
		{
			return "AntiCrisis";
		}

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> movesSet = new List<MovesSet>();

			Planets myPlanets = Context.MyStrongestPlanets(1);
			if (myPlanets.Count == 0) return movesSet;

			Planet myStrongestPlanet = myPlanets[0];

			int canSend = Context.CanSend(myStrongestPlanet);

			Planets targetFuturePlanets = new Planets(Config.MaxPlanets);
			foreach (Planet neutralPlanet in Context.NeutralPlanets())
			{
				int distance = Context.Distance(myStrongestPlanet, neutralPlanet);
				Planet futurePlanet = Context.PlanetFutureStatus(neutralPlanet, distance);

				if (futurePlanet.Owner() == 0 && canSend > futurePlanet.NumShips() + 1)
				{
					targetFuturePlanets.Add(futurePlanet);
				}
			}

			if (targetFuturePlanets.Count == 0) return movesSet;

			Comparer comparer = new Comparer(Context);
			comparer.TargetPlanet = myStrongestPlanet;
			targetFuturePlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			Moves moves = new Moves(1);
			moves.Add(new Move(myStrongestPlanet, targetFuturePlanets[0], Math.Min(canSend, targetFuturePlanets[0].NumShips() + 1)));
			movesSet.Add(new MovesSet(moves, 99999, GetAdviserName(), Context));

			return movesSet;
		}
	}
}
