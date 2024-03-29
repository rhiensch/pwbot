﻿#undef  LOG

using System;
using System.Collections.Generic;
using System.Linq;
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

			Comparer comparer = new Comparer(Context) {TargetPlanet = targetPlanet};
			myPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			PlanetHolder holder = Context.GetPlanetHolder(targetPlanet);

			foreach (Planet myPlanet in myPlanets)
			{
				int targetDistance = Context.Distance(myPlanet, targetPlanet);
				int myCanSend = Context.CanSendByPlanets(myPlanet, targetPlanet);
				if (myCanSend == 0) continue;

				Planet futurePlanet = Context.PlanetFutureStatus(targetPlanet, targetDistance);
				if (futurePlanet.Owner() != 2) continue;
				if (holder.IsNeutralToEnemySwith(targetDistance)) continue;

				int needToSend = 1 + futurePlanet.NumShips();
				if (Config.AttackSendMoreThanEnemyCanDefend)
					needToSend += Context.GetEnemyAid(targetPlanet, targetDistance);

				needToSend = moves.Aggregate(needToSend, (current, eachMove) => current - Context.CanSend(Context.GetPlanet(eachMove.SourceID)));

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
			Planets enemyPlanets = Context.Planets(); //Context.EnemyPlanets();

			List<MovesSet> movesSet = new List<MovesSet>();
			foreach (Planet planet in enemyPlanets)
			{
				PlanetHolder planetHolder = Context.GetPlanetHolder(planet);
				if ((planet.Owner() != 2) && (planetHolder.GetOwnerSwitchesToEnemy().Count == 0)) continue;


				Moves moves = Run(planet);
				if (moves.Count <= 0) continue;
				MovesSet set = new MovesSet(moves, 0, GetAdviserName(), Context);
				//double score = enemyPlanet.GrowthRate() / Context.AverageMovesDistance(moves);
				double score = 2 * planet.GrowthRate() * Config.ScoreTurns - set.NumShipsByTurns / set.AverageDistance;
				set.Score = score;

				movesSet.Add(set);
			}
			return movesSet;
		}
	}
}
