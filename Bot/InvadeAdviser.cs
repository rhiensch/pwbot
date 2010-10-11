#define DEBUG
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

			//Planet planet = SelectPlanetForAdvise();
			if (planet == null) return moves;

			Planets nearestPlanets = Context.MyPlanets();//MyPlanetsWithinProximityToPlanet(planet, Config.InvokeDistanceForInvade);
			if (nearestPlanets.Count == 0) return moves;

			
			if (nearestPlanets.Count > 1)
			{
				//nearestPlanets.Sort(new Comparer(Context).CompareNumberOfShipsGT);
				Comparer comparer = new Comparer(Context);
				comparer.TargetPlanet = planet;
				nearestPlanets.Sort(comparer.CompareDistanceToTargetPlanetLT);
			}

#if DEBUG
			Logger.Log("      Trying to invade planet " + planet.PlanetID() + "...");
#endif

			int canSend = 0;
			foreach (Planet nearestPlanet in nearestPlanets)
			{
				int distance = Context.Distance(planet, nearestPlanet);
				int extraTurns = (int)Math.Ceiling(planet.NumShips()/(double)planet.GrowthRate());
				Planet futurePlanet = Context.PlanetFutureStatus(planet, distance);
				if (futurePlanet.NumShips() == 2)
				{
					//Error?
					moves.Clear();
					return moves;
				}

				int needToSend = futurePlanet.NumShips()*(futurePlanet.Owner() == 0 ? 1 : -1);
				needToSend -= Context.GetFleetsShipNumCloserThan(Context.MyFleetsGoingToPlanet(planet), distance);
				needToSend += Context.GetEnemyAid(planet, distance + extraTurns);
				needToSend -= canSend;

				if (needToSend <= 0) return moves;

				canSend += Context.CanSend(nearestPlanet);
				Move move = new Move(nearestPlanet, planet, Math.Min(needToSend, canSend));
				if (canSend < needToSend)
				{
					//delay move
					move.TurnsBefore = 1;
				}
				moves.Add(move);
				if (canSend >= needToSend)
				{
					return moves;
				}
			}

			/*int sendedShipsNum = Context.GetFleetsShipNum(Context.MyFleetsGoingToPlanet(planet));
			int maxNeedToSend = 0;
			foreach (Planet nearPlanet in nearestPlanets)
			{
				int distance = Context.Distance(planet, nearPlanet);
				Planet futurePlanet = Context.PlanetFutureStatus(planet, distance + Config.ExtraTurns);
				int needToSend = Config.MinShipsOnPlanetsAfterInvade;
				if (futurePlanet.Owner() != 1)
				{
					needToSend += futurePlanet.NumShips();
				}
				else
				{
					needToSend -= futurePlanet.NumShips();
				}

				if (maxNeedToSend < needToSend) maxNeedToSend = needToSend;

				int canSend = Math.Min(maxNeedToSend - sendedShipsNum, Context.CanSend(nearPlanet));
				if (canSend <= 0) continue;
				moves.Add(new Move(nearPlanet.PlanetID(), planet.PlanetID(), canSend));
				sendedShipsNum += canSend;
			}
			if (sendedShipsNum < maxNeedToSend)
			{
				moves.Clear();
			}*/
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
					MovesSet set = new MovesSet(moves, score);
					movesSet.Add(set);
				}
			}
			return movesSet;
		}
	}
}
