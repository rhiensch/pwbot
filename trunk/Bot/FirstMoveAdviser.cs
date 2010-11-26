using System;
using System.Collections.Generic;
using System.Linq;
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

		public delegate bool CheckTime();

		public CheckTime CheckTimeFunc;

		public MovesSet BruteForce(Planets planets, int canSend)
		{
			Logger.Log("CanSend " + canSend);
			int scoreTurns = Config.ScoreTurns;//Context.Distance(myPlanet, enemyPlanet);

			int n = planets.Count;
			//int secondMoveCanSend = myPlanet.NumShips() - canSend + myPlanet.GrowthRate();
			int secondMoveCanSend = Math.Min(myPlanet.NumShips() - canSend + myPlanet.GrowthRate(), myPlanet.GrowthRate() * Context.Distance(myPlanet, enemyPlanet));

			List<MovesSet> sets = new List<MovesSet>();

			int size = (1 << n);
			for (int i = 0; i < size; i++)
			{
				if (CheckTimeFunc != null)
					if (!CheckTimeFunc()) break;

				int ships = 0;
				int score = 0;
				int returners = 0;
				Moves moves = new Moves();
				Planets targetPlanets = new Planets(Config.MaxPlanets);
				for (int j = 0; j < n; j++)
				{
					if (CheckTimeFunc != null)
						if (!CheckTimeFunc()) break;

					if ((i & (1 << j)) <= 0) continue;
					Planet target = planets[j];
					targetPlanets.Add(target);

					int distance = Context.Distance(myPlanet, target);
					int needShips = target.NumShips() + 1;

					if (Context.Distance(myPlanet, target) >= Context.Distance(enemyPlanet, target))
						needShips += 1;

					if (myPlanet.NumShips() > canSend && enemyDistance > distance * 2)
					{
						//HowMany ships can return to myPlanet before enemy
						returners += (enemyDistance - distance * 2) * myPlanet.GrowthRate();
						if (returners > myPlanet.NumShips() - canSend) returners = myPlanet.NumShips() - canSend;
					}

					int growTurns = Math.Max(0, scoreTurns - Context.Distance(myPlanet.PlanetID(), target.PlanetID()));

					score += growTurns * target.GrowthRate() - needShips;

					ships += needShips;
					if (ships > canSend + returners)
					{
						break;
					}
					moves.Add(
						new Move(
							myPlanet.PlanetID(),
							target.PlanetID(),
							needShips)
						);
				}
				if (ships > canSend + returners) continue;

				//clear set
				MovesSet set = new MovesSet(moves, score, GetAdviserName(), Context);
				sets.Add(set);

				//sets with additional planet, divided for 2 turns
				int firstMoveCanSend = canSend + returners - ships;
				if (firstMoveCanSend < 0) firstMoveCanSend = 0;

				foreach (Planet additionalPlanet in planets)
				{
					if (targetPlanets.Contains(additionalPlanet)) continue;

					int needShips = additionalPlanet.NumShips() + 1;
					if (Context.Distance(myPlanet, additionalPlanet) >= Context.Distance(enemyPlanet, additionalPlanet))
						needShips += 1;

					if (firstMoveCanSend + secondMoveCanSend < needShips) continue;
					//this planet we can invade in another set: this set + this planet
					if (firstMoveCanSend >= needShips) continue;

					MovesSet additionalSet = new MovesSet(set, Context);
					if (firstMoveCanSend > 0)
						additionalSet.AddMove(new Move(myPlanet, additionalPlanet, firstMoveCanSend));

					additionalSet.AddMove(new Move(myPlanet, additionalPlanet, needShips - firstMoveCanSend) { TurnsBefore = 1 });

					int growTurns = Math.Max(0, scoreTurns - Context.Distance(myPlanet.PlanetID(), additionalPlanet.PlanetID()));
					additionalSet.Score += (growTurns - 1) * additionalPlanet.GrowthRate() - needShips;

					sets.Add(additionalSet);
				}

			}
			if (sets.Count == 0) return null;
			if (sets.Count > 1)
			{
				sets.Sort(new Comparer(null).CompareSetScoreGT);
			}
			foreach (MovesSet movesSet in sets)
			{
				Logger.Log("score: " + movesSet.Score + "  " + movesSet);
			}
			return sets[0];
		}

		public override Moves Run(Planet planet)
		{
			throw new NotImplementedException();
		}

		private Planet myPlanet;
		private Planet enemyPlanet;
		private int enemyDistance;

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> setList = new List<MovesSet>();

			myPlanet = Context.MyPlanets()[0];
			enemyPlanet = Context.EnemyPlanets()[0];
			enemyDistance = Context.Distance(myPlanet, enemyPlanet);

			int canSend = Math.Min(myPlanet.NumShips(), myPlanet.GrowthRate() * Context.Distance(myPlanet, enemyPlanet));

			Planets neutralPlanets = Context.NeutralPlanets();
			Planets planets = new Planets(Config.MaxPlanets);
			planets.AddRange(neutralPlanets.Where(neutralPlanet => (Context.Distance(myPlanet, neutralPlanet) < Context.Distance(enemyPlanet, neutralPlanet)) && neutralPlanet.GrowthRate() > 0));

			setList.Add(BruteForce(planets, canSend));
			return setList;
		}

		/*private List<MovesSet> KamikadzeAttack()
		{
			List<MovesSet> setList = new List<MovesSet>(1);
			Move move = new Move(myPlanet, enemyPlanet, myPlanet.NumShips());
			Moves moves = new Moves(1) {move};

			MovesSet set = new MovesSet(moves, 0, GetAdviserName() + " kamikadze", Context);

			setList.Add(set);
			return setList;
		}

		private int GetTargetValue(Planet planet)
		{
			int score = Context.Distance(myPlanet, planet) +
				(int)Math.Ceiling((planet.NumShips()) / (double)planet.GrowthRate());
			return 200 - score;
		}

		private int GetTargetWeight(Planet planet)
		{
			int distance = Context.Distance(enemyPlanet, planet);
			int extraTurns = (int)Math.Ceiling((planet.NumShips()) / (double)planet.GrowthRate());
			int weight = Context.GetEnemyAid(planet, distance + extraTurns);

			if (weight <= planet.NumShips())
				weight = planet.NumShips() + 1;

			return weight;
		}*/

		public override string GetAdviserName()
		{
			return "FirstMove";
		}

	}
}