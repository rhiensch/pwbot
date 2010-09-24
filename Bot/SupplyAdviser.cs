using System;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace Bot
{
	public class SupplyAdviser : BaseAdviser
	{
		public Planet SupplyPlanet { get; set; }

		public SupplyAdviser(PlanetWars context)
			: base(context)
		{
		}

		public SupplyAdviser(PlanetWars context, Planet planet)
			: base(context)
		{
			SupplyPlanet = planet;
		}

		public override Moves Run()
		{
			Moves moves = new Moves();
			if (SupplyPlanet == null) return moves;

			//TODO check if planet is in danger and how many ships we can supply
			if (Context.EnemyFleetsGoingToPlanet(SupplyPlanet).Count > 0) return moves;

			if (SupplyPlanet.NumShips() == 0) return moves;

			Planets nearPlanets = Context.MyPlanetsWithinProximityToPlanet(SupplyPlanet, Config.InvokeDistanceForFront);
			if (nearPlanets.Count == 0) return moves;

			Planet dest = nearPlanets[0];
			foreach (Planet nearPlanet in nearPlanets)
			{
				if (nearPlanet.FrontLevel > dest.FrontLevel) dest = nearPlanet;
			}

			if (dest.FrontLevel > SupplyPlanet.FrontLevel)
				moves.Add(new Move(SupplyPlanet.PlanetID(), dest.PlanetID(), SupplyPlanet.NumShips()));
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Supply";
		}
	}
}
