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
			//Assert.AreEqual(Context.Distance(1, 2), Context.MyFleetsGoingToPlanet(Context.GetPlanet(2))[1].TotalTripLength());
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
		public void TestCanSendInFuture()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 1 1 1 30 5#0\n" +
				"P 9 9 1 30 5#1 we need second planet to have Router.MaxDistance > 0\n" +
				"F 2 20 1 0 5 1\n" +
				"F 2 18 1 0 5 2\n" +
				"F 2 3 1 0 5 3\n" +
				"go\n");
			int canSend = planetWars.CanSend(planetWars.GetPlanet(0), 3);

			Assert.AreEqual(4, canSend);
		}

		[TestMethod]
		public void TestCanSendShipNumber()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.613591 11.658737 0 119 0#0\n" +
				"P 1.2902863 9.040786 1 20 5#1\n" +
				"P 21.936895 14.276689 1 72 5#2\n" +
				"P 5.648358 18.265993 1 150 4#3\n" +
				"P 17.578823 5.051482 0 21 4#4\n" +
				"P 0 17.566463 1 2 2#5\n" +
				"P 23.227182 5.751012 0 32 2#6\n" +
				"P 15.9964075 22.492537 0 60 5#7\n" +
				"P 7.2307744 0.8249375 1 5 5#8\n" +
				"P 12.096861 23.317474 0 74 5#9\n" +
				"P 11.130321 0 1 6 5#10\n" +
				"P 5.9057293 2.4822736 0 85 1#11\n" +
				"P 17.321453 20.835201 0 85 1#12\n" +
				"P 18.286013 0.76577765 0 72 3#13\n" +
				"P 4.9411683 22.551697 0 72 3#14\n" +
				"P 20.10671 18.059385 2 5 5#15\n" +
				"P 3.120471 5.2580895 1 10 5#16\n" +
				"P 4.5948386 13.786 0 69 2#17\n" +
				"P 18.632343 9.531475 0 69 2#18\n" +
				"P 8.801192 20.015703 0 41 1#19\n" +
				"P 14.425989 3.3017714 0 41 1#20\n" +
				"P 19.466787 20.056168 1 39 5#21\n" +
				"P 3.7603943 3.2613065 1 5 5#22\n" +
				"F 1 10 8 3 18 1\n" +
				"F 1 10 8 3 18 2\n" +
				"F 1 5 8 3 18 3\n" +
				"F 1 9 3 15 15 1\n" +
				"F 1 5 8 3 18 4\n" +
				"F 1 10 3 15 15 2\n" +
				"F 1 20 8 3 18 5\n" +
				"F 1 11 3 15 15 3\n" +
				"F 1 20 8 3 18 6\n" +
				"F 1 11 3 15 15 4\n" +
				"F 1 11 3 15 15 5\n" +
				"F 1 57 8 4 12 3\n" +
				"F 1 5 1 3 11 4\n" +
				"F 1 5 1 3 11 5\n" +
				"F 1 5 1 3 11 6\n" +
				"F 1 2 5 3 6 1\n" +
				"F 1 15 16 1 5 1\n" +
				"F 1 5 1 3 11 7\n" +
				"F 1 2 5 3 6 2\n" +
				"F 1 2 5 3 6 3\n" +
				"F 1 15 16 1 5 2\n" +
				"F 1 5 1 3 11 8\n" +
				"F 1 20 1 3 11 9\n" +
				"F 1 2 5 3 6 4\n" +
				"F 1 10 16 1 5 3\n" +
				"F 1 5 22 16 3 1\n" +
				"F 2 54 15 2 5 4\n" +
				"F 1 5 22 16 3 2\n" +
				"F 1 10 16 1 5 4\n" +
				"F 1 2 5 3 6 5\n" +
				"F 1 20 1 3 11 10\n" +
				"F 1 88 8 10 4 3\n" +
				"go\n");
			int canSend = planetWars.CanSend(planetWars.GetPlanet(2));

			Assert.AreEqual(92-54, canSend);
		}

		[TestMethod]
		public void TestSerialization()
		{
			Assert.AreEqual(PLANETS + FLEETS + "go\n", PlanetWars.SerializeGameState(Context.Planets(), Context.Fleets()));
		}

		[TestMethod]
		public void TestSerializationForDebug()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 0 2 10 5#0\n" +
				"F 1 7 3 1 8 3\n" +
				"go\n");

			const string result =
				"\"P 0 0 2 10 5#0\\n\" +\n" +
				"\"F 1 7 3 1 8 3\\n\" +\n" +
				"\"go\\n\"\n";


			Assert.AreEqual(result, PlanetWars.SerializeGameState(pw, true));
		}

		/*[TestMethod]
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
		}*/

		[TestMethod]
		public void TestEnemyAid()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 0 1 10 5#0\n" +
				"P 1 1 2 10 5#1\n" +
				"P 3 3 2 10 5#2\n" +
				"P 5 5 2 10 5#3\n" +
				"go\n");

			Assert.AreEqual(10 + 5*4 + 10 + 5*1, pw.GetEnemyAid(pw.GetPlanet(0), 6));
		}

		/*[TestMethod]
		public void TestEnemyAidWithFleetsGoingFromFarerPlanets()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 0 1 10 5#0\n" +
				"P 1 1 2 10 5#1\n" +
				"P 3 3 2 10 5#2\n" +
				"P 5 5 2 10 5#3\n" +
				"F 2 20 1 1 8 3\n" +
				"go\n");

			Assert.AreEqual(10 + 5 * 4 + 10 + 5 * 1, pw.GetEnemyAid(pw.GetPlanet(0), 6));
		}*/

		[TestMethod]
		public void TestGetSector()
		{
			PlanetWars pw = new PlanetWars(
				"P 10.946215 11.757139 0 15 4#0\n" +
				"P 18.033072 20.11299 1 5 5#1\n" +
				"go\n");

			Assert.AreEqual(Sectors.NordEast, pw.GetSector(pw.GetPlanet(0), pw.GetPlanet(1)));
			Assert.AreEqual(Sectors.SouthWest, pw.GetSector(pw.GetPlanet(1), pw.GetPlanet(0)));
		}

		[TestMethod]
		public void TestGetSector2()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 2 1 5 0#0\n" +
				"P 1 0 1 5 0#1\n" +
				"go\n");

			Assert.AreEqual(Sectors.SouthEast, pw.GetSector(pw.GetPlanet(0), pw.GetPlanet(1)));
		}

		[TestMethod]
		public void TestGetClosestPlanetsToTargetBySectors()
		{
			PlanetWars pw = new PlanetWars(
				"P 10.946215 11.757139 0 15 4#0\n" +
				"P 18.033072 20.11299 1 5 5#1\n" +
				"P 28.033072 30.11299 1 5 5#1\n" +
				"go\n");

			Planets closestPlanets = pw.GetClosestPlanetsToTargetBySectors(pw.GetPlanet(0), pw.MyPlanets());

			Assert.AreEqual(1, closestPlanets.Count);
			Assert.AreEqual(1, closestPlanets[0].PlanetID());
		}

		[TestMethod]
		public void TestGetClosestPlanetsToTargetBySectors2()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 2 1 5 0#0\n" +
				"P 0 4 1 5 0#1\n" +
				"P 5 5 1 5 0#2\n" +
				"P 9 5 1 5 0#3\n" +
				"P 11 3 1 5 0#4\n" +
				"P 11 2 2 5 0#5\n" +
				"go\n");

			Planets closestPlanets = pw.GetClosestPlanetsToTargetBySectors(pw.GetPlanet(0), pw.MyPlanets());

			Assert.AreEqual(1, closestPlanets.Count);
			Assert.AreEqual(1, closestPlanets[0].PlanetID());
		}

		[TestMethod]
		public void TestGetClosestPlanetsToTargetBySectors3()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 2 1 5 0#0\n" +
				"P 0 4 1 5 0#1\n" +
				"P 1 0 1 5 0#2\n" +
				"P 5 1 1 5 0#3\n" +
				"P 5 5 1 5 0#4\n" +
				"P 10 0 1 5 0#5\n" +
				"P 9 5 1 5 0#6\n" +
				"P 11 3 1 5 0#7\n" +
				"P 11 2 2 5 0#8\n" +
				"go\n");

			Planets closestPlanets = pw.GetClosestPlanetsToTargetBySectors(pw.GetPlanet(0), pw.MyPlanets());

			Assert.AreEqual(2, closestPlanets.Count);
		}

		[TestMethod]
		public void TestFrontPlanets()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 0 2 15 4#0\n" +
				"P 2 0 1 5 5#1\n" +
				"P 3 0 1 5 5#2\n" +
				"P -2 0 1 5 5#3\n" +
				"go\n");

			Planets frontPlanets = pw.GetFrontPlanets();

			Assert.AreEqual(2, frontPlanets.Count);
			Assert.AreEqual(4, frontPlanets[0].PlanetID() + frontPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestFrontPlanetsNotCloseEnough()
		{
			PlanetWars pw = new PlanetWars(
				"P 0 0 2 15 4#0\n" +
				"P 2 1 1 5 5#1\n" +
				"P 3 -1 1 5 5#2\n" +
				"go\n");

			Planets frontPlanets = pw.GetFrontPlanets();

			Assert.AreEqual(1, frontPlanets.Count);
			Assert.AreEqual(1, frontPlanets[0].PlanetID());
		}
		
		[TestMethod]
		public void TestParseManyFleets()
		{
			#region
			PlanetWars pw = new PlanetWars(
				"P 0 0 2 15 4#0\n" +
				"F 2 29 12 17 14 1\n" +
				"F 1 5 9 15 13 1\n" +
				"F 2 4 8 17 13 1\n" +
				"F 1 5 9 15 13 2\n" +
				"F 1 5 9 15 13 3\n" +
				"F 1 5 9 15 13 4\n" +
				"F 1 5 9 15 13 5\n" +
				"F 1 9 1 15 8 1\n" +
				"F 1 5 9 15 13 6\n" +
				"F 2 2 3 12 8 1\n" +
				"F 2 2 3 12 8 2\n" +
				"F 1 9 1 15 8 2\n" +
				"F 1 5 9 15 13 7\n" +
				"F 1 4 6 1 6 1\n" +
				"F 1 9 1 15 8 3\n" +
				"F 1 5 9 15 13 8\n" +
				"F 2 9 16 12 6 1\n" +
				"F 2 4 5 16 6 1\n" +
				"F 2 5 2 8 6 1\n" +
				"F 2 2 3 12 8 3\n" +
				"F 1 4 6 1 6 2\n" +
				"F 1 9 1 15 8 4\n" +
				"F 1 5 9 15 13 9\n" +
				"F 2 5 10 8 6 2\n" +
				"F 2 9 16 12 6 2\n" +
				"F 2 4 5 16 6 2\n" +
				"F 2 5 2 8 6 2\n" +
				"F 2 2 3 12 8 4\n" +
				"F 2 5 10 8 6 3\n" +
				"F 2 48 12 19 4 1\n" +
				"F 2 9 16 19 4 1\n" +
				"F 2 92 8 19 7 4\n" +
				"F 2 5 2 8 6 3\n" +
				"F 2 2 3 19 6 3\n" +
				"F 2 4 5 16 6 3\n" +
				"F 1 4 6 1 6 3\n" +
				"F 1 5 9 1 7 4\n" +
				"F 1 9 1 15 8 5\n" +
				"F 1 25 4 13 6 4\n" +
				"F 1 4 6 15 6 4\n" +
				"F 1 6 1 6 6 4\n" +
				"F 1 5 9 1 7 5\n" +
				"F 2 12 12 19 4 2\n" +
				"F 2 5 10 8 6 4\n" +
				"F 2 6 19 13 9 7\n" +
				"F 2 9 16 19 4 2\n" +
				"F 2 2 3 19 6 4\n" +
				"F 2 5 2 8 6 4\n" +
				"F 2 11 8 13 6 4\n" +
				"F 2 4 5 16 6 4\n" +
				"F 2 3 12 19 4 3\n" +
				"F 2 5 10 8 6 5\n" +
				"F 2 5 19 13 9 8\n" +
				"F 2 9 16 19 4 3\n" +
				"F 2 2 3 19 6 5\n" +
				"F 2 5 2 8 6 5\n" +
				"F 2 11 8 13 6 5\n" +
				"F 2 4 5 16 6 5\n" +
				"F 1 4 6 15 6 5\n" +
				"F 1 5 1 6 6 5\n" +
				"F 1 5 9 1 7 6\n" +
				"go\n");
			#endregion

			//actually 61, but 42 grouped
			Assert.AreEqual(42, pw.Fleets().Count);

			int num = 0;
			foreach (Fleet fleet in pw.Fleets())
			{
				num += fleet.NumShips();
			}
			Assert.AreEqual(506, num);
		}
	}
}
