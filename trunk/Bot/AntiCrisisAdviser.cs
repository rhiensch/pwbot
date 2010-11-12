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

		public bool Attack { get; set; }
		
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

		//from my smallest to closest enemy
		public List<MovesSet> AttackAction()
		{
			List<MovesSet> movesSet = new List<MovesSet>();

			Planets enemyPlanets = Context.EnemyPlanets();
			if (enemyPlanets.Count == 0) return movesSet;
			enemyPlanets.Sort(new Comparer(Context).CompareGrowsRateGT);
			Planet targetPlanet = enemyPlanets[0];

			Planets myPlanets = Context.MyPlanets();
			Comparer comparer = new Comparer(Context) {TargetPlanet = targetPlanet};
			myPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			foreach (Planet myPlanet in myPlanets)
			{
				if (myPlanet.GrowthRate() < targetPlanet.GrowthRate())
				{
					Moves moves = new Moves(1)
					              	{
					              		new Move(myPlanet, targetPlanet, Context.CanSend(myPlanet))
					              	};
					movesSet.Add(new MovesSet(moves, 99999, GetAdviserName(), Context));
					break;
				}
			}

			return movesSet;
		}

		//From my strongest to closest suitable enemy
		public List<MovesSet> InvadeAction()
		{
			List<MovesSet> movesSet = new List<MovesSet>();

			Planets myPlanets = Context.MyStrongestPlanets(1);
			if (myPlanets.Count == 0) return movesSet;

			Planet myStrongestPlanet = myPlanets[0];

			int canSend = Context.CanSend(myStrongestPlanet);

			Planets targetFuturePlanets = new Planets(Config.MaxPlanets);
			foreach (Planet eachPlanet in Context.Planets())
			{
				if (eachPlanet.GrowthRate() == 0) continue;

				int distance = Context.Distance(myStrongestPlanet, eachPlanet);
				Planet futurePlanet = Context.PlanetFutureStatus(eachPlanet, distance);

				if (futurePlanet.Owner() == 0 && canSend > futurePlanet.NumShips() + 1)
				{
					targetFuturePlanets.Add(futurePlanet);
				}
			}

			if (targetFuturePlanets.Count == 0) return movesSet;

			Comparer comparer = new Comparer(Context) { TargetPlanet = myStrongestPlanet };
			targetFuturePlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			Moves moves = new Moves(1)
			              	{
			              		new Move(myStrongestPlanet, targetFuturePlanets[0],
			              		         Math.Min(canSend, targetFuturePlanets[0].NumShips() + 1))
			              	};
			movesSet.Add(new MovesSet(moves, 99999, GetAdviserName(), Context));

			return movesSet;
		}

		public override List<MovesSet> RunAll()
		{
			if (Attack) return AttackAction();
			return InvadeAction();
		}
	}
}
