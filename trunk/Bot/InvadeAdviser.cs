#undef DEBUG
using System;
using System.Collections.Generic;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class InvadeAdviser : BaseAdviser
	{
		public InvadeAdviser(PlanetWars context)
			: base(context)
		{
		}

		/*private Planet SelectPlanetForAdvise()
		{
			if (Context.MyProduction > Context.EnemyProduction * Config.DoInvadeKoef)
			{
				IsWorkFinished = true;
				return null;
			}

			Planets neutralPlanets = Context.NeutralPlanets();

			if (usedPlanets.Count > 0)
			{
				foreach (Planet usedPlanet in usedPlanets)
				{
					int index = neutralPlanets.IndexOf(usedPlanet);
					if (index != -1) neutralPlanets.RemoveAt(index);
				}
			}

			if (neutralPlanets.Count == 0)
			{
				IsWorkFinished = true;
				return null;
			}

			Planets neutralPlanetsSuitable = new Planets(neutralPlanets);
			foreach (Planet neutralPlanet in neutralPlanets)
			{
				if (neutralPlanet.GrowthRate() == 0)
				{
					usedPlanets.Add(neutralPlanet);
					neutralPlanetsSuitable.Remove(neutralPlanet);
					continue;
				}

				Planets nearestPlanets = Context.MyPlanetsWithinProximityToPlanet(neutralPlanet, Config.InvokeDistanceForInvade);
				if (nearestPlanets.Count == 0)
				{
					usedPlanets.Add(neutralPlanet);
					neutralPlanetsSuitable.Remove(neutralPlanet);
				}
			}

			if (neutralPlanetsSuitable.Count == 0)
			{
				Config.IncInvadeDistance();
				IsWorkFinished = true;
				return null;
			}
			Config.ResetInvadeDistance();

			if (neutralPlanetsSuitable.Count == 1)
			{
				usedPlanets.Add(neutralPlanetsSuitable[0]);
				return neutralPlanets[0];
			}

			neutralPlanetsSuitable.Sort(new Comparer(Context).CompareImportanceOfNeutralPlanetsGT);
			usedPlanets.Add(neutralPlanetsSuitable[0]);
			
			return neutralPlanetsSuitable[0];
		}*/

		public override Moves Run(Planet planet)
		{
			Moves moves = new Moves();

			if (planet == null) return moves;

			Planets nearestPlanets = Context.MyPlanets();//MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);
			if (nearestPlanets.Count == 0) return moves;
			
			if (nearestPlanets.Count > 1)
			{
				Comparer comparer = new Comparer(Context);
				comparer.TargetPlanet = planet;
				nearestPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);
			}

			Fleets myFleetsGoingToPlanet = Context.MyFleetsGoingToPlanet(planet);
			int farestFleet = PlanetWars.GetFarestFleetDistance(myFleetsGoingToPlanet);

			int sendedShips = 0;
			foreach (Planet nearestPlanet in nearestPlanets)
			{
				int canSend = Context.CanSend(nearestPlanet);
				if (canSend == 0) continue;

				int distance = Context.Distance(planet, nearestPlanet);
				int extraTurns = (int)Math.Ceiling(planet.NumShips()/(double)planet.GrowthRate());
				Planet futurePlanet = Context.PlanetFutureStatus(planet, distance);
				if (futurePlanet.NumShips() == 2)//Error?
				{
					moves.Clear();
					return moves;
				}

				int myFleetsShipNum = Context.GetFleetsShipNumFarerThan(myFleetsGoingToPlanet, distance);

				int needToSend = 1 + futurePlanet.NumShips()*(futurePlanet.Owner() == 0 ? 1 : -1);
				needToSend -= myFleetsShipNum;//Context.GetFleetsShipNumCloserThan(Context.MyFleetsGoingToPlanet(planet), distance);
				if (Config.InvadeSendMoreThanEnemyCanDefend)
				{
					needToSend += Context.GetEnemyAid(planet, distance + extraTurns);
				}
				needToSend -= sendedShips;

				if (needToSend <= 0) return moves;

				sendedShips += canSend;
				Move move = new Move(nearestPlanet, planet, Math.Min(needToSend, sendedShips));
				moves.Add(move);
				if (sendedShips >= needToSend)
				{
					//delay closer moves
					foreach (Move eachMove in moves)
					{
						int moveDistance = Context.Distance(eachMove.DestinationID, eachMove.SourceID);
						int maxDistance = Math.Max(distance, farestFleet);
						eachMove.TurnsBefore = maxDistance - moveDistance;
					}
					return moves;
				}
			}

			moves.Clear();
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Invade";
		}

		public override List<MovesSet> RunAll()
		{
			List<MovesSet> movesSet = new List<MovesSet>();
			Planets planetsForAdvise = Context.NeutralPlanets();
			foreach (Planet planet in planetsForAdvise)
			{
				if (planet.GrowthRate() == 0) continue;

				PlanetHolder planetHolder = Context.GetPlanetHolder(planet);
				if (planetHolder.GetOwnerSwitchesFromNeutralToEnemy().Count > 0) continue;

				Moves moves = Run(planet);
				if (moves.Count > 0)
				{
					double score = planet.GrowthRate() / (Context.AverageMovesDistance(moves) + planet.NumShips());
					MovesSet set = new MovesSet(moves, score, GetAdviserName());
					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
