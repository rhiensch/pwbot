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

		public override Moves Run(Planet planet)
		{
			Moves moves = new Moves();

			if (planet == null) return moves;

			Planets nearestPlanets = Context.MyPlanets();//MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);
			if (nearestPlanets.Count == 0) return moves;
			
			if (nearestPlanets.Count > 1)
			{
				Comparer comparer = new Comparer(Context);
				comparer.TargetPlanet = planet;
				nearestPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);
			}

			Fleets myFleetsGoingToPlanet = Context.MyFleetsGoingToPlanet(planet);
			int farestFleet = PlanetWars.GetFarestFleetDistance(myFleetsGoingToPlanet);

			foreach (Planet nearestPlanet in nearestPlanets)
			{
				int canSend = Context.CanSend(nearestPlanet);
				if (canSend == 0) continue;

				int distance = Context.Distance(planet, nearestPlanet);

				int sendedShips = 0;
				//delay closer moves
				foreach (Move eachMove in moves)
				{
					int moveDistance = Context.Distance(eachMove.DestinationID, eachMove.SourceID);
					int maxDistance = Math.Max(distance, farestFleet);
					int turns = maxDistance - moveDistance;
					eachMove.TurnsBefore = turns;
					sendedShips += Context.CanSend(Context.GetPlanet(eachMove.SourceID), turns);
				}

				int extraTurns = (int)Math.Ceiling(planet.NumShips()/(double)planet.GrowthRate());
				Planet futurePlanet = Context.PlanetFutureStatus(planet, distance);
				if (futurePlanet.NumShips() == 2)//Error?
				{
					moves.Clear();
					return moves;
				}

				int myFleetsShipNum = Context.GetFleetsShipNumFarerThan(myFleetsGoingToPlanet, distance);

				int needToSend = futurePlanet.Owner() == 0 ? 1 + futurePlanet.NumShips() : -futurePlanet.NumShips();
				needToSend -= myFleetsShipNum;
				if (Config.InvadeSendMoreThanEnemyCanDefend)
				{
					needToSend += Context.GetEnemyAid(planet, distance + extraTurns);
				}
				needToSend -= sendedShips;

				if (needToSend <= 0) return moves;

				sendedShips += canSend;
				Move move = new Move(nearestPlanet, planet, Math.Min(needToSend, sendedShips));
				moves.Add(move);
				if (sendedShips >= needToSend) return moves;
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
					double score = Config.ScoreKoef * risk * (planet.GrowthRate() / (set.AverageDistance * 100 + planet.NumShips()));
					set.Score = score;

					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
