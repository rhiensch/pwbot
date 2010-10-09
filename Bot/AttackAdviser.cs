#define  DEBUG

using System;
using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class AttackAdviser : BaseAdviser
	{
		public AttackAdviser(PlanetWars context)
			: base(context)
		{
		}

		/*private Planet internalTargetPlanet;
		public Planet TargetPlanet
		{
			get { return internalTargetPlanet; }
			private set { internalTargetPlanet = value; }
		}

		private Planet SelectPlanetForAdvise()
		{
			Planets enemyPlanets = Context.EnemyPlanets();

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = enemyPlanets.IndexOf(usedPlanet);
					if (index != -1) enemyPlanets.RemoveAt(index);
				}
			}

			if (enemyPlanets.Count == 0)
			{
				IsWorkFinished = true;
				return null;
			}
			if (enemyPlanets.Count == 1)
			{
				usedPlanets.Add(enemyPlanets[0]);
				return enemyPlanets[0];
			}

			int minDistance = int.MaxValue;
			Planet targetPlanet = null;
			foreach (Planet enemyPlanet in enemyPlanets)
			{
				int distance = Context.GetClosestMyPlanetDistance(enemyPlanet);
				if (distance < minDistance)
				{
					minDistance = distance;
					targetPlanet = enemyPlanet;
				}
			}

			usedPlanets.Add(targetPlanet);
			TargetPlanet = targetPlanet;
			return targetPlanet;
		}*/

		public override Moves Run(Planet targetPlanet)
		{
			Moves moves = new Moves();
			//Planet targetPlanet = SelectPlanetForAdvise();
			if (targetPlanet == null) return moves;

			Planets myPlanets = Context.MyPlanets();
			if (myPlanets.Count == 0) return moves;

			Comparer comparer = new Comparer(Context);
			comparer.TargetPlanet = targetPlanet;
			myPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			Fleets addMyFleets = new Fleets();
			Fleets addEnemyFleets = new Fleets();

			int bestMark = int.MaxValue;
			int bestMovesCount = 0;

			foreach (Planet myPlanet in myPlanets)
			{
				int targetDistance = Context.Distance(myPlanet, targetPlanet);
				int myCanSend = Context.CanSend(myPlanet);

				if (myCanSend > 0)
				{
					Move move = new Move(myPlanet, targetPlanet, myCanSend);
					moves.Add(move);
					addMyFleets.Add(Context.MoveToFleet(1, move));
				}

				Planets defendPlanets = Context.PlanetsWithinProximityToPlanet(
											Context.EnemyPlanets(),
											targetPlanet,
											targetDistance);

				Moves defendMoves = Context.GetPossibleDefendMoves(targetPlanet, defendPlanets, targetDistance);

				addEnemyFleets.Clear();
				foreach (Move defendMove in defendMoves)
				{
					addEnemyFleets.Add(Context.MoveToFleet(2, defendMove));
				}

				Fleets addFleets = new Fleets(addMyFleets);
				addFleets.AddRange(addEnemyFleets);
				Planet futurePlanet = 
					Context.PlanetFutureStatus(targetPlanet, targetDistance, addFleets);
				if (futurePlanet.Owner() == 1) return moves; //Victory!

				Planet futurePlanetWithoutDefend =
					Context.PlanetFutureStatus(targetPlanet, targetDistance, addMyFleets);

				if (futurePlanetWithoutDefend.Owner() == 1)
				{
					if (bestMark > futurePlanet.NumShips())
					{
						bestMark = futurePlanet.NumShips();
						bestMovesCount = addMyFleets.Count;
					}
				}
			}

			/*if (bestMovesCount > 0)
			{
				moves = moves.GetRange(0, bestMovesCount);
				return moves;
			}*/

			return new Moves();
		}

		public override string GetAdviserName()
		{
			return "Attack";
		}

		public override List<MovesSet> RunAll()
		{
			Planets enemyPlanets = Context.EnemyPlanets();

			List<MovesSet> movesSet = new List<MovesSet>();
			foreach (Planet enemyPlanet in enemyPlanets)
			{
				Moves moves = Run(enemyPlanet);
				if (moves.Count > 0)
				{
					MovesSet set = new MovesSet(moves, enemyPlanet.GrowthRate());
					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
