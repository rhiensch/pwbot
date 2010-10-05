using System;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;
using PlanetsCombination = System.Collections.Generic.List<System.Collections.Generic.List<Bot.Planet>>;
using PlanetsCombinations = System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<Bot.Planet>>>;

namespace Bot
{
	public class FirstMoveAdviser : BaseAdviser
	{
		public FirstMoveAdviser(PlanetWars context)
			: base(context)
		{
		}

		public static Planets GetTargetPlanets(Planets candidates, int canSend)
		{
			PlanetsCombination allCombination = new PlanetsCombination();
			int maximum = 1 << candidates.Count;
			for (int i = 0; i < maximum; i++)
			{
				Planets combination = new Planets();
				int set = i;
				for (int j = 0; j < candidates.Count; j++, set >>= 1)
					if ((set & 0x01) == 1)
						combination.Add(candidates[j]);
				allCombination.Add(combination);
			}

			Planets bestTargets = null;
			int bestScore = 0;
			foreach (Planets planets in allCombination)
			{
				int score = 0;
				int sheepsNeeded = 0;
				foreach (Planet planet in planets)
				{
					sheepsNeeded += planet.NumShips() + 1;
					if (sheepsNeeded > canSend) break;
					score += planet.GrowthRate();
				}
				if (sheepsNeeded > canSend) continue;
				if (score > bestScore)
				{
					bestScore = score;
					bestTargets = planets;
				}
			}

			return bestTargets;
		}

		/*private PlanetsCombination GetPlanetCombination(Planets candidates, int n)
		{
			PlanetsCombination combination = new PlanetsCombination();

			if (n == 1)
			{
				foreach (Planet candidate in candidates)
				{
					Planets planets = new Planets();
					planets.Add(candidate);
					combination.Add(planets);
				}
				
				return combination;
			}

			foreach (Planet candidate in candidates)
			{
				combination.Add(candidate);
			}

			return combination;
		}*/

		public override Moves Run()
		{
			Moves moves = new Moves();

			Planet myPlanet = Context.MyPlanets()[0];
			Planet enemyPlanet = Context.EnemyPlanets()[0];

			int canSend = Math.Min(myPlanet.NumShips(), myPlanet.GrowthRate() * Context.Distance(myPlanet, enemyPlanet));

			Planets neutralPlanets = Context.NeutralPlanets();
			Planets planets = new Planets();
			foreach (Planet neutralPlanet in neutralPlanets)
			{
				if (Context.Distance(myPlanet, neutralPlanet) < 
					Context.Distance(enemyPlanet, neutralPlanet))
				{
					planets.Add(neutralPlanet);
				}
			}

			Planets targetPlanets = GetTargetPlanets(planets, canSend);
			int sendedShips = 0;
			foreach (Planet targetPlanet in targetPlanets)
			{
				sendedShips += targetPlanet.NumShips() + 1;
				if (sendedShips > myPlanet.NumShips()) break; //ERROR!

				Move move = new Move(myPlanet, targetPlanet, targetPlanet.NumShips() + 1);
				moves.Add(move);
			}

			return moves;
		}

		public override string GetAdviserName()
		{
			return "FirstMove";
		}
	}
}
