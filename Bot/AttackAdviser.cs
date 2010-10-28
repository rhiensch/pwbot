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

			//Fleets myFleetsGoingToPlanet = Context.MyFleetsGoingToPlanet(targetPlanet);
			//int farestFleet = PlanetWars.GetFarestFleetDistance(myFleetsGoingToPlanet);

			foreach (Planet myPlanet in myPlanets)
			{
				int targetDistance = Context.Distance(myPlanet, targetPlanet);
				int myCanSend = Context.CanSend(myPlanet);
				if (myCanSend == 0) continue;

				Planet futurePlanet = Context.PlanetFutureStatus(targetPlanet, targetDistance);
				if (futurePlanet.Owner() != 2) continue;

				//int myFleetsShipNum = Context.GetFleetsShipNumFarerThan(myFleetsGoingToPlanet, targetDistance);
				//targetDistance = Math.Max(targetDistance, farestFleet);

				int needToSend = 1 + futurePlanet.NumShips();
				//needToSend -= myFleetsShipNum;
				needToSend += Context.GetEnemyAid(targetPlanet, targetDistance);

				foreach (Move eachMove in moves)
				{
					needToSend -= Context.CanSend(Context.GetPlanet(eachMove.SourceID));
				}
				//delay closer moves
				/*foreach (Move eachMove in moves)
				{
					int moveDistance = Context.Distance(eachMove.DestinationID, eachMove.SourceID);
					int turns = targetDistance - moveDistance;
					eachMove.TurnsBefore = turns;
					needToSend -= Context.CanSend(Context.GetPlanet(eachMove.SourceID), turns);
				}*/

				if (needToSend <= 0) return moves;

				int canSend = Math.Min(needToSend, myCanSend);
				needToSend -= canSend;
				Move move = new Move(myPlanet, targetPlanet, canSend);
				moves.Add(move);

				if (needToSend <= 0) return moves;
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
			foreach (Planet planet in enemyPlanets)
			{
				Moves moves = Run(planet);
				if (moves.Count > 0)
				{
					MovesSet set = new MovesSet(moves, 0, GetAdviserName(), Context);
					//double score = enemyPlanet.GrowthRate() / Context.AverageMovesDistance(moves);
					double score = 2 * planet.GrowthRate() * Config.ScoreTurns - set.NumShipsByTurns;
					set.Score = score;

					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
