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
				
				weights.Add(GetTargetWeight(p));
				
				values.Add(GetTargetValue(p));

				int distance = Context.Distance(enemyPlanet, p);
				int extraTurns = (int) Math.Ceiling((p.NumShips())/(double) p.GrowthRate());
				Context.GetEnemyAid(p, distance + extraTurns);
				Logger.Log(
					"planet: " + p +
					" value: " + GetTargetValue(p) +
					" distance: " + Context.Distance(myPlanet, p) +
					" weight: " + GetTargetWeight(p)+
					" canSend: " + Context.GetEnemyAid(p, distance + extraTurns));
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

		public override Moves Run(Planet planet)
		{
			throw new NotImplementedException();
		}

		private Planet myPlanet;
		private Planet enemyPlanet;

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> setList = new List<MovesSet>();

			myPlanet = Context.MyPlanets()[0];
			enemyPlanet = Context.EnemyPlanets()[0];

			//int canSend = Math.Min(myPlanet.NumShips(), myPlanet.GrowthRate() * Context.Distance(myPlanet, enemyPlanet));
			int canSend = myPlanet.NumShips();
			int distance = Context.Distance(myPlanet, enemyPlanet);
			if (distance <= 5)
			{
				//kamikadze attack
				return KamikadzeAttack();
			}

			if (distance < 10)
			{
				canSend = Math.Min(myPlanet.NumShips(), myPlanet.GrowthRate() * Context.Distance(myPlanet, enemyPlanet));
			}

			Planets neutralPlanets = Context.NeutralPlanets();
			Planets planets = new Planets();
			foreach (Planet neutralPlanet in neutralPlanets)
			{
				if (Context.Distance(myPlanet, neutralPlanet) <=
					Context.Distance(enemyPlanet, neutralPlanet))
				{
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

				int ships = GetTargetWeight(targetPlanet);

				Move move = new Move(myPlanet, targetPlanet, ships);
				Moves moves = new Moves(1);
				moves.Add(move);

				//Logger.Log("move" + move);

				MovesSet set = new MovesSet(moves, GetTargetValue(targetPlanet), GetAdviserName(), Context);

				setList.Add(set);
			}

			return setList;
		}

		private List<MovesSet> KamikadzeAttack()
		{
			List<MovesSet> setList = new List<MovesSet>(1);
			Move move = new Move(myPlanet, enemyPlanet, myPlanet.NumShips());
			Moves moves = new Moves(1);
			moves.Add(move);

			MovesSet set = new MovesSet(moves, 0, GetAdviserName() + " kamikadze", Context);

			setList.Add(set);
			return setList;
		}

		private int GetTargetValue(Planet planet)
		{
			/*double score = planet.GrowthRate() * Config.ScoreTurns - 
				planet.NumShips() * Context.Distance(myPlanet, planet) - 
				planet.NumShips();*/
			int score = Context.Distance(myPlanet, planet) + 
				(int)Math.Ceiling((planet.NumShips()) / (double)planet.GrowthRate());
			return 200 - score;
		}

		private int GetTargetWeight(Planet planet)
		{
			int distance = Context.Distance(enemyPlanet, planet);
			int extraTurns = (int) Math.Ceiling((planet.NumShips())/(double) planet.GrowthRate());
			int weight = Context.GetEnemyAid(planet, distance + extraTurns);

			if (weight <= planet.NumShips())
				weight = planet.NumShips() + 1;

			return weight;
		}

		public override string GetAdviserName()
		{
			return "FirstMove";
		}

	}
}
