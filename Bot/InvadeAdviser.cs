#undef LOG
using System;
using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class InvadeAdviser : BaseAdviser
	{
		public InvadeAdviser(PlanetWars context)
			: base(context)
		{
		}

		public override Moves Run(Planet targetPlanet)
		{
			Moves moves = new Moves();

			if (targetPlanet == null) return moves;

			Planets nearestPlanets = Context.GetClosestPlanetsToTargetBySectors(targetPlanet, Context.MyPlanets());
									//Context.MyPlanets();
									//MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);););
			if (nearestPlanets.Count == 0) return moves;
			
			if (nearestPlanets.Count > 1)
			{
				Comparer comparer = new Comparer(Context) {TargetPlanet = targetPlanet};
				nearestPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);
			}

			foreach (Planet nearestPlanet in nearestPlanets)
			{
				int canSend = Context.CanSendByPlanets(nearestPlanet, targetPlanet);
				if (canSend == 0) continue;

				int distance = Context.Distance(targetPlanet, nearestPlanet);

				Planet futurePlanet = Context.PlanetFutureStatus(targetPlanet, distance);
				if (futurePlanet.Owner() == 2)//Error?
				{
#if LOG
					Logger.Log("InvadeAdvizer: Error?");
#endif
					moves.Clear();
					return moves;
				}

				int needToSend = futurePlanet.NumShips() + 1;
				if (Config.InvadeSendMoreThanEnemyCanDefend)
				{
					int extraTurns = (int)Math.Ceiling(targetPlanet.NumShips() / (double)targetPlanet.GrowthRate());
					if (Context.MyFutureProduction < Context.EnemyFutureProduction) extraTurns = 0;
					if ((Context.MyFutureProduction == Context.EnemyFutureProduction) &&
						(Context.MyTotalShipCount <= Context.EnemyTotalShipCount)) extraTurns = 0;
					needToSend += Context.GetEnemyAid(targetPlanet, distance + extraTurns);
				}

				foreach (Move eachMove in moves)
				{
					needToSend -= Context.CanSendByPlanets(Context.GetPlanet(eachMove.SourceID), Context.GetPlanet(eachMove.DestinationID));
				}
				/*
				//delay closer moves
				foreach (Move eachMove in moves)
				{
					int moveDistance = Context.Distance(eachMove.DestinationID, eachMove.SourceID);
					int turns = distance - moveDistance;
					eachMove.TurnsBefore = turns;
					needToSend -= Context.CanSend(Context.GetPlanet(eachMove.SourceID), turns);
				}*/

				if (needToSend <= 0) return moves;

				canSend = Math.Min(needToSend, canSend);
				needToSend -= canSend;
				Move move = new Move(nearestPlanet, targetPlanet, canSend);
				moves.Add(move);

				if (needToSend <= 0) return moves;
			}

			moves.Clear();
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Invade";
		}

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> movesSet = new List<MovesSet>();
			Planets planetsForAdvise = Context.NeutralPlanets();
			foreach (Planet planet in planetsForAdvise)
			{
				if (planet.GrowthRate() == 0) continue;

				PlanetHolder planetHolder = Context.GetPlanetHolder(planet);
				//if (planetHolder.GetOwnerSwitchesToEnemy().Count > 0) continue;
				if (planetHolder.OwnerSwitches.Count > 0) continue;

				Moves moves = Run(planet);
				if (moves.Count <= 0) continue;
				MovesSet set = new MovesSet(moves, 0, GetAdviserName(), Context);

				/*int enemyAid = Context.GetEnemyAid(planet, set.MaxDistance);
					double risk = 2.0;
					if (enemyAid != 0) risk = set.SummaryNumShips / (double)enemyAid;
					double score = Config.ScoreKoef * risk * (planet.GrowthRate() / (set.MaxDistance * 100.0 + planet.NumShips()));*/
				double score = planet.GrowthRate() * Config.ScoreTurns - set.NumShipsByTurns - planet.NumShips();
				set.Score = score;

				movesSet.Add(set);
			}
			return movesSet;
		}
	}
}
