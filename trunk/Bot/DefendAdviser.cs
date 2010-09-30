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

		private Planet SelectPlanetForAdvise()
		{
			Planets myPlanetsUnderAttack = Context.MyPlanetsUnderAttack();
			//myPlanetsUnderAttack.AddRange(Context.MyInvasionNeutralPlanetsUnderAttack());

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = myPlanetsUnderAttack.IndexOf(usedPlanet);
					if (index != -1) myPlanetsUnderAttack.RemoveAt(index);
				}
			}

			if (myPlanetsUnderAttack.Count == 0)
			{
				IsWorkFinished = true;
				return null;
			}
			if (myPlanetsUnderAttack.Count == 1)
			{
				usedPlanets.Add(myPlanetsUnderAttack[0]);
				return myPlanetsUnderAttack[0];
			}

			myPlanetsUnderAttack.Sort(new Comparer(Context).CompareImportanceOfPlanetsGT);
			usedPlanets.Add(myPlanetsUnderAttack[0]);
			return myPlanetsUnderAttack[0];
		}

		public override Moves Run()
		{
			Moves moves = new Moves();

			Planet planet = SelectPlanetForAdvise();
			if (planet == null) return moves;

			List<Pair<int, int>> saveSteps =
					Context.GetMyPlanetSaveSteps(planet, Config.MinShipsOnPlanetsAfterDefend);

			if (saveSteps.Count == 0) return moves;

			for (int i = 0; i < saveSteps.Count; i++)
			{
				Planets planetsCanHelp = Context.MyPlanetsWithinProximityToPlanet(planet, saveSteps[i].First);
				planetsCanHelp.Sort(new Comparer(Context).CompareNumberOfShipsGT);

				int sendedShipsNum = 0;
				foreach (Planet nearPlanet in planetsCanHelp)
				{
					int canSend = Math.Min(saveSteps[i].Second - sendedShipsNum, Context.CanSend(nearPlanet));
					if (canSend <= 0) continue;
					moves.Add(new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend));
					sendedShipsNum += canSend;
				}
			}

			return moves;
		}

		public override string GetAdviserName()
		{
			return "Defend";
		}
	}
}
