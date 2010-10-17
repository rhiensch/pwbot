#define DEBUG
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

			Planets nearestPlanets = Context.GetClosestPlanetsToTargetBySectors(targetPlanet, Context.MyPlanets());//Context.MyPlanets();//MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);
			if (nearestPlanets.Count == 0) return moves;
			
			if (nearestPlanets.Count > 1)
			{
				Comparer comparer = new Comparer(Context);
				comparer.TargetPlanet = targetPlanet;
				nearestPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);
			}

			Fleets myFleetsGoingToPlanet = Context.MyFleetsGoingToPlanet(targetPlanet);
			int farestFleet = PlanetWars.GetFarestFleetDistance(myFleetsGoingToPlanet);

			foreach (Planet nearestPlanet in nearestPlanets)
			{
				int canSend = Context.CanSend(nearestPlanet);
				if (canSend == 0) continue;

				int distance = Context.Distance(targetPlanet, nearestPlanet);
				int maxDistance = Math.Max(distance, farestFleet);
				if (targetPlanet.PlanetID() == 13) Logger.Log("max " + maxDistance);

				Planet futurePlanet = Context.PlanetFutureStatus(targetPlanet, maxDistance);
				if (futurePlanet.NumShips() == 2)//Error?
				{
#if DEBUG
					Logger.Log("InvadeAdvizer: Error?");
#endif
					moves.Clear();
					return moves;
				}

				int needToSend = 0;
				if (futurePlanet.Owner() != 1)
					needToSend += 1 + futurePlanet.NumShips();
				if (Config.InvadeSendMoreThanEnemyCanDefend)
				{
					int extraTurns = (int)Math.Ceiling(targetPlanet.NumShips() / (double)targetPlanet.GrowthRate());
					needToSend += Context.GetEnemyAid(targetPlanet, maxDistance + extraTurns);
					//if (targetPlanet.PlanetID() == 0)
					//	Logger.Log("target planet: " + targetPlanet + " turns" + (maxDistance + extraTurns) + " Aid: " + Context.GetEnemyAid(targetPlanet, maxDistance + extraTurns));
				}
				//delay closer moves
				foreach (Move eachMove in moves)
				{
					int moveDistance = Context.Distance(eachMove.DestinationID, eachMove.SourceID);
					int turns = maxDistance - moveDistance;
					eachMove.TurnsBefore = turns;
					needToSend -= Context.CanSend(Context.GetPlanet(eachMove.SourceID), turns);
				}

				//int myFleetsShipNum = Context.GetFleetsShipNumFarerThan(myFleetsGoingToPlanet, distance);
				//needToSend -= myFleetsShipNum;
				if (futurePlanet.NumShips() == 1)
					needToSend -= futurePlanet.NumShips();

				if (needToSend <= 0) return moves;

				canSend = Math.Min(needToSend, canSend);
				needToSend -= canSend;
				Move move = new Move(nearestPlanet, targetPlanet, canSend);
				move.TurnsBefore = maxDistance - distance;
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
				if (planetHolder.GetOwnerSwitchesFromNeutralToEnemy().Count > 0) continue;

				Moves moves = Run(planet);
				if (moves.Count > 0)
				{
					MovesSet set = new MovesSet(moves, 0, GetAdviserName(), Context);

					int enemyAid = Context.GetEnemyAid(planet, set.MaxDistance);
					double risk = 2.0;
					if (enemyAid != 0) risk = set.SummaryNumShips / (double)enemyAid;
					double score = Config.ScoreKoef * risk * (planet.GrowthRate() / (set.MaxDistance * 100.0 + planet.NumShips()));
					set.Score = score;

					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
