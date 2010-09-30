#undef DEBUG

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
			myPlanets.Sort(comparer.CompareDistanceToTargetPlanetGT);

			int sendedShipsNum = Context.GetFleetsShipNum(Context.MyFleetsGoingToPlanet(planet));
			int maxNeedToSend = 0;
			foreach (Planet myPlanet in myPlanets)
			{
				int distance = Context.Distance(planet, myPlanet);
				int needToSend = Context.PlanetFutureStatus(planet, distance).NumShips() + Config.MinShipsOnPlanetsAfterAttack;

				int canSend = Math.Min(needToSend - sendedShipsNum, Context.CanSend(myPlanet));
				if (canSend <= 0) continue;

				if (canSend < Context.CanSend(myPlanet)) canSend = Context.CanSend(myPlanet);

				moves.Add(new Move(myPlanet.PlanetID(), planet.PlanetID(), canSend));
				sendedShipsNum += canSend;

				if (maxNeedToSend < needToSend) maxNeedToSend = needToSend;

				if (sendedShipsNum >= maxNeedToSend) break;
			}
			if (sendedShipsNum < maxNeedToSend)
			{
				moves.Clear();
#if DEBUG
				//Logger.Log("    ...failed");
#endif
			} 
			else
			{
#if DEBUG
				//Logger.Log(moves.Count == 0 ? "    no need to send" : "    accepted!");
#endif
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
