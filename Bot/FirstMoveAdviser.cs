using System;
using System.Collections.Generic;
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

		public Planets Knapsack01(List<Planet> planets, int maxWeight)
		{
			List<int> weights = new List<int>();
			List<int> values = new List<int>();

			// solve 0-1 knapsack problem  
			foreach (Planet p in planets)
			{
				// here weights and values are numShips and growthRate respectively 
				// you can change this to something more complex if you like...
				weights.Add(p.NumShips() + 1);
				values.Add(GetTargetScore(p));
			}

			int[,] knapsack = new int[weights.Count + 1, maxWeight];

			int i;
			for (i = 0; i < maxWeight; i++)
			{
				knapsack[0, i] = 0;
			}
			for (int k = 1; k <= weights.Count; k++)
			{
				for (int y = 1; y <= maxWeight; y++)
				{
					if (y < weights[k - 1]) knapsack[k, y - 1] = knapsack[k - 1, y - 1];
					else if (y > weights[k - 1]) knapsack[k, y - 1] = Math.Max(knapsack[k - 1, y - 1], knapsack[k - 1, y - 1 - weights[k - 1]] + values[k - 1]);
					else knapsack[k, y - 1] = Math.Max(knapsack[k - 1, y - 1], values[k - 1]);
				}
			}

			// get the planets in the solution
			i = weights.Count;
			int currentW = maxWeight - 1;
			Planets markedPlanets = new Planets();

			while ((i > 0) && (currentW >= 0))
			{
				if (((i == 0) && (knapsack[i, currentW] > 0)) || (knapsack[i, currentW] != knapsack[i - 1, currentW]))
				{
					markedPlanets.Add(planets[i - 1]);
					currentW = currentW - weights[i - 1];
				}
				i--;
			}
			return markedPlanets;
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

		public override Moves Run(Planet planet)
		{
			throw new NotImplementedException();
		}

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> setList = new List<MovesSet>();

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
					Logger.Log("planet" + neutralPlanet + " distance from my =" + Context.Distance(myPlanet, neutralPlanet) + " distance from enemy = " + Context.Distance(enemyPlanet, neutralPlanet));
					planets.Add(neutralPlanet);
				}
			}

			Planets targetPlanets = Knapsack01(planets, canSend);
				//GetTargetPlanets(planets, canSend);
			int sendedShips = 0;
			foreach (Planet targetPlanet in targetPlanets)
			{
				sendedShips += targetPlanet.NumShips() + 1;
				if (sendedShips > myPlanet.NumShips()) break; //ERROR!

				Move move = new Move(myPlanet, targetPlanet, targetPlanet.NumShips() + 1);
				Moves moves = new Moves(1);
				moves.Add(move);

				//Logger.Log("move" + move);

				MovesSet set = new MovesSet(moves, GetTargetScore(targetPlanet), GetAdviserName());

				setList.Add(set);
			}

			return setList;
		}

		private int GetTargetScore(Planet planet)
		{
			double score = planet.GrowthRate()/(double)(Context.Distance(Context.MyPlanets()[0], planet));
			return Convert.ToInt32(score * 1000);
		}

		public override string GetAdviserName()
		{
			return "FirstMove";
		}

	}
}
