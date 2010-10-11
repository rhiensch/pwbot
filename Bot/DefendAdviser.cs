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

		/*private Planet SelectPlanetForAdvise()
		{
			Planets myEndangeredPlanets = Context.MyEndangeredPlanets();
			//myPlanetsUnderAttack.AddRange(Context.MyInvasionNeutralPlanetsUnderAttack());

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = myEndangeredPlanets.IndexOf(usedPlanet);
					if (index != -1) myEndangeredPlanets.RemoveAt(index);
				}
			}

			if (myEndangeredPlanets.Count == 0)
			{
				IsWorkFinished = true;
				return null;
			}
			if (myEndangeredPlanets.Count == 1)
			{
				usedPlanets.Add(myEndangeredPlanets[0]);
				return myEndangeredPlanets[0];
			}

			myEndangeredPlanets.Sort(new Comparer(Context).CompareImportanceOfPlanetsGT);
			usedPlanets.Add(myEndangeredPlanets[0]);
			return myEndangeredPlanets[0];
		}*/

		public override Moves Run(Planet planet)
		{
			Moves moves = new Moves();

			//Planet planet = SelectPlanetForAdvise();
			if (planet == null) return moves;

			List<Step> saveSteps = Context.GetMyPlanetSaveSteps(planet);

			if (saveSteps.Count == 0) return moves;

			for (int i = 0; i < saveSteps.Count; i++)
			{
				Planets planetsCanHelp = Context.MyPlanetsWithinProximityToPlanet(planet, saveSteps[i].ToTurn);

				Comparer comparer = new Comparer(Context);
				comparer.TargetPlanet = planet;
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
					}
					moves.Add(move);
					sendedShipsNum += canSend;
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
				if (moves.Count > 0)
				{
					double score = planet.GrowthRate()/Context.AverageMovesDistance(moves);
					MovesSet set = new MovesSet(moves, score);
					movesSet.Add(set);
				}
			}
			return movesSet;
		}

		public override string GetAdviserName()
		{
			return "Defend";
		}
	}
}
