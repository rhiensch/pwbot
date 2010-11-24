using System;
using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class DefendAdviser : BaseAdviser
	{
		public DefendAdviser(PlanetWars context)
			: base(context)
		{
		}

		private int loseTurn;
		public override Moves Run(Planet planet)
		{
			Moves moves = new Moves();
			loseTurn = 0;

			//Planet planet = SelectPlanetForAdvise();
			if (planet == null) return moves;

			List<Step> saveSteps = Context.GetMyPlanetSaveSteps(planet);

			if (saveSteps.Count == 0) return moves;

			foreach (Step t in saveSteps)
			{
				Planets planetsCanHelp = Context.MyPlanetsWithinProximityToPlanet(planet, t.ToTurn);

				Comparer comparer = new Comparer(Context) {TargetPlanet = planet};
				planetsCanHelp.Sort(comparer.CompareDistanceToTargetPlanetLT);

				int sendedShipsNum = 0;
				foreach (Planet nearPlanet in planetsCanHelp)
				{
					int canSend = Math.Min(t.NumShips - sendedShipsNum, Context.CanSendByPlanets(nearPlanet, planet));
					if (canSend <= 0) continue;

					int distance = Context.Distance(planet, nearPlanet);
					Move move = new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend);
					if (distance < t.ToTurn)
					{
						//delay move
						move.TurnsBefore = t.ToTurn - distance;
						//move = new Move(nearPlanet.PlanetID(), planet.PlanetID(), Context.CanSend(nearPlanet, move.TurnsBefore));
					}
					moves.Add(move);
					sendedShipsNum += canSend;
				}
				if (sendedShipsNum < t.NumShips)
				{
					loseTurn = t.NumShips;
				}
			}

			return moves;
		}

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> movesSet = new List<MovesSet>();
			Planets planetsForAdvise = Context.MyEndangeredPlanets();
			foreach (Planet planet in planetsForAdvise)
			{
				Moves moves = Run(planet);
				if (moves.Count <= 0) continue;
				MovesSet set = new MovesSet(moves, 0, GetAdviserName(), Context);
				//double score = enemyPlanet.GrowthRate() / Context.AverageMovesDistance(moves);
				double score = 2 * planet.GrowthRate() * (loseTurn > 0 ? loseTurn : Config.ScoreTurns) - set.NumShipsByTurns / set.AverageDistance;
				set.Score = score;

				if (loseTurn == 0) 
					movesSet.Add(set);
			}
			return movesSet;
		}

		public override string GetAdviserName()
		{
			return "Defend";
		}
	}
}







/*


 */