#define  DEBUG

using System;
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

		private Planet SelectPlanetForAdvise()
		{
			Planets enemyPlanets = Context.EnemyPlanets();

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = enemyPlanets.IndexOf(usedPlanet);
					if (index != -1) enemyPlanets.RemoveAt(index);
				}
			}

			if (enemyPlanets.Count == 0)
			{
				IsWorkFinished = true;
				return null;
			}
			if (enemyPlanets.Count == 1)
			{
				usedPlanets.Add(enemyPlanets[0]);
				return enemyPlanets[0];
			}

			enemyPlanets.Sort(new Comparer(Context).CompareImportanceOfEnemyPlanetsGT);
			usedPlanets.Add(enemyPlanets[0]);
			return enemyPlanets[0];
		}

		public override Moves Run()
		{
			Moves moves = new Moves();
			Planet planet = SelectPlanetForAdvise();
			if (planet == null) return moves;

			Planets myPlanets = Context.MyPlanets();
			if (myPlanets.Count == 0) return moves;

			Comparer comparer = new Comparer(Context);
			comparer.TargetPlanet = planet;
			myPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			int sendedShipsNum = 0;
			int maxNeedToSend = 0;
			foreach (Planet myPlanet in myPlanets)
			{
				int distance = Context.Distance(planet, myPlanet);

				Planet futurePlanet = Context.PlanetFutureStatus(planet, distance);
				if (futurePlanet.Owner() != 2) return moves; //Error?

				int needToSend = futurePlanet.NumShips() + Config.MinShipsOnPlanetsAfterAttack;

				int canSend = Context.CanSend(myPlanet);
				if (canSend <= 0) continue;

				moves.Add(new Move(myPlanet.PlanetID(), planet.PlanetID(), canSend));
				sendedShipsNum += canSend;

				if (maxNeedToSend < needToSend) maxNeedToSend = needToSend;

				Planets enemyDefenders = Context.PlanetsWithinProximityToPlanet(Context.EnemyPlanets(), planet, distance);
				int defenders = Context.GetPlanetsShipNum(enemyDefenders);

				if (sendedShipsNum >= maxNeedToSend + defenders) return moves;
			}
			moves.Clear();
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Attack";
		}
	}
}
