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

			Planets nearPlanets = Context.MyPlanets();
			if (nearPlanets.Count == 0) return moves;

			Planets frontPlanets = new Planets();
			foreach (Planet nearPlanet in nearPlanets)
			{
				int nearPlanetSumDistance = Context.GetPlanetSummaryDistance(Context.EnemyPlanets(), nearPlanet);

				if (nearPlanetSumDistance < supplyPlanetSumDistance) frontPlanets.Add(nearPlanet);
			}

			if (frontPlanets.Count == 0) return moves;

			int minDistance = int.MaxValue;
			Planet dest = null;
			foreach (Planet frontPlanet in frontPlanets)
			{
				int distance = Context.Distance(frontPlanet, SupplyPlanet);
				if (minDistance > distance)
				{
					minDistance = distance;
					dest = frontPlanet;
				}
			}

			if (dest != null)
				moves.Add(new Move(SupplyPlanet.PlanetID(), dest.PlanetID(), SupplyPlanet.NumShips()));
			return moves;
		}

		public override string GetAdviserName()
		{
			return "Supply";
		}
	}
}
