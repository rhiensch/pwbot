using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class SupplyAdviser : BaseAdviser
	{
		private Planet supplyPlanet;
		public Planet SupplyPlanet
		{
			get { return supplyPlanet; }
			set { supplyPlanet = value; }
		}

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

			int supplyPlanetSumDistance = Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), SupplyPlanet);

			Planets nearPlanets = Context.MyPlanetsWithinProximityToPlanet(SupplyPlanet, Config.InvokeDistanceForFront);
			if (nearPlanets.Count == 0) return moves;

			Planet dest = nearPlanets[0];
			int minSumDistance = int.MaxValue;
			foreach (Planet nearPlanet in nearPlanets)
			{
				int nearPlanetSumDistance = Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), nearPlanet);

				if (nearPlanetSumDistance < minSumDistance)
				{
					dest = nearPlanet;
					minSumDistance = nearPlanetSumDistance;
				}
				//if (nearPlanet.FrontLevel > dest.FrontLevel) dest = nearPlanet;
			}

			//if (dest.FrontLevel > SupplyPlanet.FrontLevel)
			if (minSumDistance < supplyPlanetSumDistance)
				moves.Add(new Move(SupplyPlanet.PlanetID(), dest.PlanetID(), SupplyPlanet.NumShips()));
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Supply";
		}
	}
}
