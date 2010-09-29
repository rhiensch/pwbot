#define DEBUG

using System;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class AttackAdviser : BaseAdviser
	{
		public AttackAdviser(PlanetWars context)
			: base(context)
		{
		}

		private Planet SelectPlanetForAttack()
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

			if (enemyPlanets.Count == 0) return null;
			if (enemyPlanets.Count == 1) return enemyPlanets[0];

			enemyPlanets.Sort(new Comparer(Context).CompareImportanceOfEnemyPlanetsGT);
			return enemyPlanets[0];
		}

		public override Moves Run()
		{
			Moves moves = new Moves();
			Planet planet = SelectPlanetForAttack();
			if (planet == null) return moves;

			Planets myPlanets = Context.MyPlanets();
			if (myPlanets.Count == 0) return moves;

			Comparer comparer = new Comparer(Context);
			comparer.TargetPlanet = planet;
			myPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			int sendedShipsNum = Context.GetFleetsShipNum(Context.MyFleetsGoingToPlanet(planet));
			int maxNeedToSend = 0;
			foreach (Planet nearPlanet in myPlanets)
			{
				//TODO maybe check for endangered?
				if (Context.EnemyFleetsGoingToPlanet(nearPlanet).Count > 0) continue;

				int distance = Context.Distance(planet, nearPlanet);
				int needToSend = Context.PlanetFutureStatus(planet, distance).NumShips() + Config.MinShipsOnMyPlanetsAfterAttack;

				int canSend = Math.Min(needToSend - sendedShipsNum, nearPlanet.NumShips() - Config.MinShipsOnMyPlanetsAfterAttack);
				if (canSend <= 0) continue;
				moves.Add(new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend));
				sendedShipsNum += canSend;

				if (maxNeedToSend < needToSend) maxNeedToSend = needToSend;

				if (sendedShipsNum >= maxNeedToSend) break;
			}
			if (sendedShipsNum < maxNeedToSend)
			{
				moves.Clear();
#if DEBUG
				Logger.Log("    ...failed");
#endif
			} 
			else
			{
#if DEBUG
				Logger.Log(moves.Count == 0 ? "    no need to send" : "    accepted!");
#endif
				usedPlanets.Add(planet);
				return moves;
			}

			return moves;
		}

		public override string GetAdviserName()
		{
			return "Attack";
		}
	}
}
