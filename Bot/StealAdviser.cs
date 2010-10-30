using System;
using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class StealAdviser : BaseAdviser
	{
		public StealAdviser(PlanetWars context)
			: base(context)
		{
		}

		public override Moves Run(Planet stealPlanet)
		{
			Moves moves = new Moves();

			PlanetHolder planetHolder = Context.GetPlanetHolder(stealPlanet);
			List<PlanetOwnerSwitch> switches = planetHolder.GetOwnerSwitchesFromNeutralToEnemy();
			if (switches.Count == 0) return moves;

			Planet futurePlanet = null;
			int turn = 0;
			for (int i = 0; i < switches.Count; i++)
			{
				turn = switches[i].TurnsBefore + 1;
				futurePlanet = Context.PlanetFutureStatus(stealPlanet, turn);
				if (futurePlanet.Owner() != 1) break;
				futurePlanet = null;
			}
			if (futurePlanet == null) return moves;
			
			Logger.Log("Steal " + Context.GetPlanet(futurePlanet.PlanetID()));

			Planets myPlanets = Context.MyPlanetsWithinProximityToPlanet(stealPlanet, turn);
			if (myPlanets.Count == 0) return moves;

			int needToSend = futurePlanet.NumShips() + 1;
			//needToSend += Context.GetEnemyAid(stealPlanet, turn);

			foreach (Planet myPlanet in myPlanets)
			{
				int canSend = Context.CanSend(myPlanet, turn);
				if (canSend == 0) continue;

				int send = Math.Min(canSend, needToSend);
				needToSend -= send;

				Move move = new Move(myPlanet, stealPlanet, send);
				int distance = Context.Distance(myPlanet, stealPlanet);
				if (distance < turn) move.TurnsBefore = turn - distance;
				moves.Add(move);
				

				if (needToSend <= 0) return moves;
			}

			moves.Clear();
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Steal";
		}

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> movesSet = new List<MovesSet>();
			Planets planetsForAdvise = Context.NeutralPlanets();
			foreach (Planet planet in planetsForAdvise)
			{
				if (planet.GrowthRate() == 0) continue;

				PlanetHolder planetHolder = Context.GetPlanetHolder(planet);
				if (planetHolder.GetOwnerSwitchesFromNeutralToEnemy().Count == 0) continue;

				Moves moves = Run(planet);
				if (moves.Count > 0)
				{
					MovesSet set = new MovesSet(moves, 0, GetAdviserName(), Context);
					double score = 2 * planet.GrowthRate() * Config.ScoreTurns - set.NumShipsByTurns;
					set.Score = score;

					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
