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

		public override Moves Run(Planet targetPlanet)
		{
			Moves moves = new Moves();
			if (targetPlanet == null) return moves;

			Planets myPlanets = Context.MyPlanets();
			if (myPlanets.Count == 0) return moves;

			Comparer comparer = new Comparer(Context);
			comparer.TargetPlanet = targetPlanet;
			myPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			Fleets myFleetsGoingToPlanet = Context.MyFleetsGoingToPlanet(targetPlanet);
			int farestFleet = PlanetWars.GetFarestFleetDistance(myFleetsGoingToPlanet);

			int sendedShips = 0;
			foreach (Planet myPlanet in myPlanets)
			{
				int targetDistance = Context.Distance(myPlanet, targetPlanet);
				int myCanSend = Context.CanSend(myPlanet);
				if (myCanSend == 0) continue;

				Planet futurePlanet = Context.PlanetFutureStatus(targetPlanet, targetDistance);
				if (futurePlanet.Owner() != 2) continue;

				int myFleetsShipNum = Context.GetFleetsShipNumFarerThan(myFleetsGoingToPlanet, targetDistance);
				targetDistance = Math.Max(targetDistance, farestFleet);

				int needToSend = 1 + futurePlanet.NumShips();
				needToSend -= myFleetsShipNum;
				needToSend += Context.GetEnemyAid(targetPlanet, targetDistance);
				if (targetPlanet.PlanetID() == 16) Logger.Log("EnemyAid : " + Context.GetEnemyAid(targetPlanet, targetDistance) + " distance: " + targetDistance);
				needToSend -= sendedShips;

				if (needToSend <= 0) return moves;

				sendedShips += Math.Min(myCanSend, needToSend);

				Move move = new Move(myPlanet, targetPlanet, Math.Min(myCanSend, needToSend));
				moves.Add(move);
				if (sendedShips >= needToSend)
				{
					//delay closer moves
					foreach (Move eachMove in moves)
					{
						int moveDistance = Context.Distance(eachMove.DestinationID, eachMove.SourceID);
						int maxDistance = targetDistance;
						eachMove.TurnsBefore = maxDistance - moveDistance;
					}
					return moves;
				}
			}

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
					double score = enemyPlanet.GrowthRate() / Context.AverageMovesDistance(moves);
					MovesSet set = new MovesSet(moves, score, GetAdviserName(), Context);
					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
