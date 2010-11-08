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

			for (int i = 0; i < saveSteps.Count; i++)
			{
				Planets planetsCanHelp = Context.MyPlanetsWithinProximityToPlanet(planet, saveSteps[i].ToTurn);

				Comparer comparer = new Comparer(Context) {TargetPlanet = planet};
				planetsCanHelp.Sort(comparer.CompareDistanceToTargetPlanetLT);

				int sendedShipsNum = 0;
				foreach (Planet nearPlanet in planetsCanHelp)
				{
					int canSend = Math.Min(saveSteps[i].NumShips - sendedShipsNum, Context.CanSend(nearPlanet));
					if (canSend <= 0) continue;

					int distance = Context.Distance(planet, nearPlanet);
					Move move = new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend);
					if (distance < saveSteps[i].ToTurn)
					{
						//delay move
						move.TurnsBefore = saveSteps[i].ToTurn - distance;
						//move = new Move(nearPlanet.PlanetID(), planet.PlanetID(), Context.CanSend(nearPlanet, move.TurnsBefore));
					}
					moves.Add(move);
					sendedShipsNum += canSend;
				}
				if (sendedShipsNum < saveSteps[i].NumShips)
				{
					loseTurn = saveSteps[i].NumShips;
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
				double score = 2 * planet.GrowthRate() * (loseTurn > 0 ? loseTurn :Config.ScoreTurns) - set.NumShipsByTurns;
				set.Score = score;

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