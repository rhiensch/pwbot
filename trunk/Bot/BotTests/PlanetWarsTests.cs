using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bot;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace BotTests
{
	[TestClass]
	public class PlanetWarsTests
	{
		public PlanetWars Context { get; set; }

		[TestInitialize]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;

			CreateTestContextForSort();
		}

		private const string PLANETS =
			"P 11.6135908004 11.6587374197 0 119 0#0\n" +
			"P 1.2902863101 9.04078582767 1 40 5#1\n" +
			"P 21.9368952907 14.2766890117 2 100 5#2\n" +
			"P 5.64835767563 18.2659924733 1 21 4#3\n" +
			"P 17.5788239251 5.05148236609 0 21 4#4\n";

		private const string FLEETS =
			"F 1 25 1 4 5 3\n" +
			"F 1 50 1 2 10 1\n" +
			"F 2 30 2 4 5 2\n" +
			"F 2 60 2 1 15 1\n";

		public string CreateTestMessageForSort()
		{
			return PLANETS + FLEETS + "go\n";
		}

		public PlanetWars CreateTestContextForSort()
		{
			Context = new PlanetWars(CreateTestMessageForSort());
			return Context;
		}

		[TestMethod]
		public void TestSingleWeakestPlanet()
		{
			Planets singleWeakestPlanet = Context.WeakestPlanets(Context.Planets(), 1);

			Assert.AreEqual(1, singleWeakestPlanet.Count);
			Assert.AreEqual(3, singleWeakestPlanet[0].PlanetID());
		}

		[TestMethod]
		public void TestSomeWeakestPlanets()
		{
			Planets singleWeakestPlanet = Context.WeakestPlanets(Context.Planets(), 3);

			Assert.AreEqual(3, singleWeakestPlanet.Count);
			Assert.IsTrue((singleWeakestPlanet[0].PlanetID() == 3) || (singleWeakestPlanet[0].PlanetID() == 4));
			Assert.IsTrue((singleWeakestPlanet[1].PlanetID() == 3) || (singleWeakestPlanet[1].PlanetID() == 4));
			Assert.IsTrue((singleWeakestPlanet[2].PlanetID() == 1) || (singleWeakestPlanet[2].PlanetID() == 2));
			
		}

		[TestMethod]
		public void TestSingleStrongestPlanet()
		{
			List<Planet> singleStrongestPlanet = Context.StrongestPlanets(Context.Planets(), 1);

			Assert.AreEqual(1, singleStrongestPlanet.Count);
			Assert.AreEqual(0, singleStrongestPlanet[0].PlanetID());
		}

		[TestMethod]
		public void TestSomeStrongestPlanets()
		{
			Planets strongestPlanets = Context.StrongestPlanets(Context.Planets(), 3);

			Assert.AreEqual(3, strongestPlanets.Count);
			Assert.AreEqual(0, strongestPlanets[0].PlanetID());
			Assert.IsTrue((strongestPlanets[1].PlanetID() == 1) || (strongestPlanets[1].PlanetID() == 2));
			Assert.IsTrue((strongestPlanets[2].PlanetID() == 1) || (strongestPlanets[2].PlanetID() == 2));
		}

		[TestMethod]
		public void TestMyWeakestPlanets()
		{
			Planets weakestPlanets = Context.MyWeakestPlanets(2);

			Assert.AreEqual(2, weakestPlanets.Count);
			Assert.AreEqual(3, weakestPlanets[0].PlanetID());
			Assert.AreEqual(1, weakestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestMyStrongestPlanets()
		{
			Planets strongestPlanets = Context.MyStrongestPlanets(2);

			Assert.AreEqual(2, strongestPlanets.Count);
			Assert.AreEqual(1, strongestPlanets[0].PlanetID());
			Assert.AreEqual(3, strongestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestNeutralWeakestPlanets()
		{
			Planets weakestPlanets = Context.NeutralWeakestPlanets(2);

			Assert.AreEqual(2, weakestPlanets.Count);
			Assert.AreEqual(4, weakestPlanets[0].PlanetID());
			Assert.AreEqual(0, weakestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestNeutralStrongestPlanets()
		{
			Planets strongestPlanets = Context.NeutralStrongestPlanets(3);

			Assert.AreEqual(2, strongestPlanets.Count);
			Assert.AreEqual(0, strongestPlanets[0].PlanetID());
			Assert.AreEqual(4, strongestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestEnemyWeakestPlanets()
		{
			Planets weakestPlanets = Context.EnemyWeakestPlanets(2);

			Assert.AreEqual(1, weakestPlanets.Count);
			Assert.AreEqual(2, weakestPlanets[0].PlanetID());
		}

		[TestMethod]
		public void TestEnemyStrongestPlanets()
		{
			Planets strongestPlanets = Context.EnemyStrongestPlanets(3);

			Assert.AreEqual(1, strongestPlanets.Count);
			Assert.AreEqual(2, strongestPlanets[0].PlanetID());
		}

		[TestMethod]
		public void TestPlanetsWithGivenOwner()
		{
			Planets neutralPlanets = Context.PlanetsWithGivenOwner(Context.Planets(), 0);

			Assert.AreEqual(2, neutralPlanets.Count);
			Assert.IsTrue((neutralPlanets[0].PlanetID() == 0) || (neutralPlanets[0].PlanetID() == 4));
			Assert.IsTrue((neutralPlanets[1].PlanetID() == 0) || (neutralPlanets[1].PlanetID() == 4));
		}

		[TestMethod]
		public void TestFleetsWithGivenOwner()
		{
			Fleets myFleets = PlanetWars.FleetsWithGivenOwner(Context.Fleets(), 1);

			Assert.AreEqual(2, myFleets.Count);
			Assert.AreEqual(75, myFleets[0].NumShips() + myFleets[1].NumShips());
		}

		[TestMethod]
		public void TestFleetsGoingToPlanet()
		{
			Fleets attackingFleets = Context.FleetsGoingToPlanet(Context.Fleets(), Context.GetPlanet(4));

			Assert.AreEqual(2, attackingFleets.Count);
			Assert.AreEqual(55, attackingFleets[0].NumShips() + attackingFleets[1].NumShips());

			attackingFleets = Context.MyFleetsGoingToPlanet(Context.GetPlanet(4));

			Assert.AreEqual(1, attackingFleets.Count);
			Assert.AreEqual(25, attackingFleets[0].NumShips());

			attackingFleets = Context.EnemyFleetsGoingToPlanet(Context.GetPlanet(4));

			Assert.AreEqual(1, attackingFleets.Count);
			Assert.AreEqual(30, attackingFleets[0].NumShips());
		}

		[TestMethod]
		public void TestPlanetsUnderAttack()
		{
			Planets planets = Context.MyPlanetsUnderAttack();

			Assert.AreEqual(1, planets.Count);
			Assert.AreEqual(1, planets[0].PlanetID());

			planets = Context.NeutralPlanetsUnderAttack();

			Assert.AreEqual(1, planets.Count);
			Assert.AreEqual(4, planets[0].PlanetID());
		}

		[TestMethod]
		public void TestPlanetsWithinProximityToPlanet()
		{
			Planets nearPlanets = Context.PlanetsWithinProximityToPlanet(Context.Planets(), Context.GetPlanet(1), 11);

			Assert.AreEqual(2, nearPlanets.Count);
			Assert.IsTrue((nearPlanets[0].PlanetID() == 0) || (nearPlanets[0].PlanetID() == 3));
			Assert.IsTrue((nearPlanets[1].PlanetID() == 0) || (nearPlanets[1].PlanetID() == 3));
		}

		[TestMethod]
		public void TestPlanetFutureStatus()
		{
			Planet futurePlanet = Context.PlanetFutureStatus(Context.GetPlanet(4), 3);

			Assert.AreEqual(12, futurePlanet.NumShips());
			Assert.AreEqual(1, futurePlanet.Owner());

			futurePlanet = Context.PlanetFutureStatus(Context.GetPlanet(4), 2);

			Assert.AreEqual(9, futurePlanet.NumShips());
			Assert.AreEqual(2, futurePlanet.Owner());
		}

		[TestMethod]
		public void TestMyEndangeredPlanets()
		{
			Planets planets = Context.MyEndangeredPlanets();

			Assert.AreEqual(1, planets.Count);
			Assert.AreEqual(1, planets[0].PlanetID());
		}

		[TestMethod]
		public void TestPlanetSummaryDistance()
		{
			int distance = Context.GetPlanetSummaryDistance(Context.MyPlanets(), Context.GetPlanet(1));

			Assert.AreEqual(Context.Distance(1, 3), distance);

			distance = Context.GetPlanetSummaryDistance(Context.NeutralPlanets(), Context.GetPlanet(1));

			Assert.AreEqual(Context.Distance(1, 0) + Context.Distance(1, 4), distance);
		}

		[TestMethod]
		public void TestMakeMove()
		{
			Move move = new Move(1, 2, 20);

			Context.IssueOrder(move);

			Assert.AreEqual(5, Context.Fleets().Count);
			Assert.AreEqual(3, Context.MyFleets().Count);
			Assert.AreEqual(40-20, Context.GetPlanet(1).NumShips());
			Assert.AreEqual(Context.Distance(1, 2), Context.MyFleetsGoingToPlanet(Context.GetPlanet(2))[1].TotalTripLength());
			Assert.AreEqual(Context.Distance(1, 2), Context.MyFleetsGoingToPlanet(Context.GetPlanet(2))[1].TurnsRemaining());
		}

		[TestMethod]
		public void TestProduction()
		{
			Assert.AreEqual(9, Context.Production(1));
			Assert.AreEqual(9, Context.MyProduction);
			Assert.AreEqual(5, Context.Production(2));
			Assert.AreEqual(5, Context.EnemyProduction);
			Assert.AreEqual(0, Context.Production(0));
		}

		[TestMethod]
		public void TestTotalShipCount()
		{
			Assert.AreEqual(40 + 21 + 25 + 50, Context.TotalShipCount(1));
			Assert.AreEqual(40 + 21 + 25 + 50, Context.MyTotalShipCount);
			Assert.AreEqual(100 + 30 + 60, Context.TotalShipCount(2));
			Assert.AreEqual(100 + 30 + 60, Context.EnemyTotalShipCount);
			Assert.AreEqual(119 + 21, Context.TotalShipCount(0));
		}

		[TestMethod]
		public void TestCanSend()
		{
			int canSend = Context.CanSend(Context.GetPlanet(3));
			Assert.AreEqual(Context.GetPlanet(3).NumShips(), canSend);

			canSend = Context.CanSend(Context.GetPlanet(1));
			Assert.AreEqual(0, canSend);

			PlanetWars planetWars = new PlanetWars(
				"P 1 1 1 30 5#0\n" +
				"P 9 9 1 30 5#1 we need second planet to have Router.MaxDistance > 0\n" +
				"F 2 20 1 0 5 1\n" +
				"F 2 18 1 0 5 2\n" +
				"F 2 3 1 0 5 3\n" +
				"go\n");
			canSend = planetWars.CanSend(planetWars.GetPlanet(0));

			Assert.AreEqual(2, canSend);
		}

		[TestMethod]
		public void TestSerialization()
		{
			Assert.AreEqual(PLANETS + FLEETS + "go\n", PlanetWars.SerializeGameState(Context.Planets(), Context.Fleets()));
		}

		[TestMethod]
		public void TestGetPossibleDefendMoves()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 0 2 10 5#0\n" +
				"P 1 1 2 10 5#1\n" +
				"P 3 3 2 10 5#2\n" +
				"P 5 5 1 10 5#3\n" +
				"F 1 7 3 1 8 3\n" +
				"F 1 8 3 1 8 4\n" +
				"F 2 2 2 1 4 2\n" +
				"go\n");

			Moves moves = pw.GetPossibleDefendMoves(pw.GetPlanet(0), pw.EnemyPlanets(), 5);

			Assert.AreEqual(2, moves.Count);
			Assert.AreEqual(1, moves[0].SourceID);
			Assert.AreEqual(3, moves[0].TurnsBefore);
			Assert.AreEqual(10 + 3 * 5 - 7 + 2, moves[0].NumSheeps);

			Assert.AreEqual(2, moves[1].SourceID);
			Assert.AreEqual(0, moves[1].TurnsBefore);
			Assert.AreEqual(10, moves[1].NumSheeps);
		}
	}
}
