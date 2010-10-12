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

			int turns = switches[0].TurnsBefore + 1;

			Planets myPlanets = Context.MyPlanetsWithinProximityToPlanet(stealPlanet, turns);
			if (myPlanets.Count == 0) return moves;

			Planet futurePlanet = Context.PlanetFutureStatus(stealPlanet, turns);
			if (futurePlanet.Owner() < 2) return moves;

			int needToSend = futurePlanet.NumShips() + 1;
			needToSend += Context.GetEnemyAid(stealPlanet, turns);

			int sendedShips = 0;
			foreach (Planet myPlanet in myPlanets)
			{
				int canSend = Context.CanSend(myPlanet);
				if (canSend == 0) continue;

				Move move = new Move(myPlanet, stealPlanet, canSend);
				int distance = Context.Distance(myPlanet, stealPlanet);
				if (distance < turns)
				{
					move.TurnsBefore = turns - distance;
				}
				moves.Add(move);

				sendedShips += canSend;

				if (sendedShips > needToSend) return moves;
			}

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
					double score = planet.GrowthRate() / Context.AverageMovesDistance(moves);
					MovesSet set = new MovesSet(moves, score, GetAdviserName());
					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
