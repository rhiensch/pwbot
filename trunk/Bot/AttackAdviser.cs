using System;
using System.Collections.Generic;
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

		public override Moves Run()
		{
			Moves moves = new Moves();

			Planets enemyPlanets = Context.EnemyPlanets();
			if (enemyPlanets.Count == 0) return moves;

			enemyPlanets.Sort(new Comparer(Context).CompareImportanceOfEnemyPlanetsGT);

			foreach (Planet planet in enemyPlanets)
			{
				Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForAttack);
				if (nearestPlanets.Count == 0) continue;

				nearestPlanets.Sort(new Comparer(Context).CompareNumberOfShipsGT);

				int sendedShipsNum = Context.GetFleetsShipNum(Context.MyFleetsGoingToPlanet(planet));
				int maxNeedToSend = 0;
				foreach (Planet nearPlanet in nearestPlanets)
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
				}
				if (sendedShipsNum < maxNeedToSend) moves.Clear(); else break;
			}

			return moves;
		}
	}
}
