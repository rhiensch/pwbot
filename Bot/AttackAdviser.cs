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

		private Planets usedPlanets;

		public override Moves Run()
		{
			Moves moves = new Moves();

			if (usedPlanets == null) usedPlanets = new Planets();

			Planets enemyPlanets = Context.EnemyPlanets();
			if (enemyPlanets.Count == 0) return moves;

			enemyPlanets.Sort(new Comparer(Context).CompareImportanceOfEnemyPlanetsGT);

			foreach (Planet planet in enemyPlanets)
			{
				if (usedPlanets.IndexOf(planet) != -1) continue;

#if DEBUG
				Logger.Log("    Trying to attack " + planet.PlanetID() + "...");
#endif
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
			}

			return moves;
		}

		public override string GetAdviserName()
		{
			return "Attack";
		}
	}
}
