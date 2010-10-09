using System;
using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class SupplyAdviser : BaseAdviser
	{
		public SupplyAdviser(PlanetWars context)
			: base(context)
		{
		}

		/*private Planet testSupplyPlanet;

		public SupplyAdviser(PlanetWars context, Planet supplyPlanet)
			: base(context)
		{
			SupplyPlanet = supplyPlanet;
		}

		public Planet SupplyPlanet
		{
			get { return testSupplyPlanet; }
			set { testSupplyPlanet = value; }
		}

		//private int iter;
		private Planet SelectPlanetForAdvise()
		{
			//iter++;
			Planets myPlanets = Context.MyPlanets();

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = myPlanets.IndexOf(usedPlanet);
					if (index != -1) myPlanets.RemoveAt(index);
				}
			}

			if (myPlanets.Count == 0)
			//if (iter == myPlanets.Count)
			{
				IsWorkFinished = true;
				return null;
			}

			usedPlanets.Add(myPlanets[0]);
			return myPlanets[0];
		}*/

		public override Moves Run(Planet supplyPlanet)
		{
			Moves moves = new Moves();

			//Planet supplyPlanet = SupplyPlanet/* ?? SelectPlanetForAdvise()*/;
			if (supplyPlanet == null) return moves;

			if (Context.EnemyPlanets().Count == 0)
			{
				IsWorkFinished = true;
				return moves;
			}

			int canSend = Context.CanSend(supplyPlanet);
			if (canSend == 0) return moves;

			int supplyPlanetFrontLevel = Context.GetClosestEnemyPlanetDistance(supplyPlanet);
				//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), supplyPlanet);

			Planets nearPlanets = Context.MyPlanets();
			if (nearPlanets.Count == 0) return moves;

			Planets frontPlanets = new Planets();
			foreach (Planet nearPlanet in nearPlanets)
			{
				int nearPlanetFrontLevel = Context.GetClosestEnemyPlanetDistance(nearPlanet);
					//Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), nearPlanet);

				if (nearPlanetFrontLevel < supplyPlanetFrontLevel)
				{
					frontPlanets.Add(nearPlanet);
				}
			}

			if (frontPlanets.Count == 0) return moves;

			int minDistance = int.MaxValue;
			Planet dest = null;
			foreach (Planet frontPlanet in frontPlanets)
			{
				int distance = Context.Distance(frontPlanet, supplyPlanet);
				if (minDistance > distance)
				{
					minDistance = distance;
					dest = frontPlanet;
				}
			}

			if (dest != null)
			{
				moves.Add(new Move(supplyPlanet.PlanetID(), dest.PlanetID(), canSend));
			}

			return moves;
		}

		public override string GetAdviserName()
		{
			return "Supply";
		}

		public override List<MovesSet> RunAll()
		{
			Planets myPlanets = Context.MyPlanets();

			List<MovesSet> movesSet = new List<MovesSet>();
			foreach (Planet myPlanet in myPlanets)
			{
				Moves moves = Run(myPlanet);
				if (moves.Count > 0)
				{
					MovesSet set = new MovesSet(moves, 0);
					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
