using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bot;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace BotTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class BaseBotTests
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		// Use ClassInitialize to run code before running the first test in the class
		/*[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}*/

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		[TestInitialize]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		public string CreateTestMessageForSort()
		{
			return 
				"P 11.6135908004 11.6587374197 0 119 0#0\n" +
				"P 1.2902863101 9.04078582767 1 40 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 5.64835767563 18.2659924733 1 21 4#3\n" +
				"P 17.5788239251 5.05148236609 0 21 4#4\n" +
				"F 1 25 1 4 5 3\n" +
				"F 1 50 1 2 10 1\n" +
				"F 2 30 2 4 5 2\n" +
				"F 2 60 2 1 15 1\n" +
				"go\n";
		}

		public PlanetWars CreateTestContextForSort()
		{
			return new PlanetWars(CreateTestMessageForSort());
		}

		[TestMethod]
		public void TestSingleWeakestPlanet()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets singleWeakestPlanet = bot.WeakestPlanets(bot.Context.Planets(), 1);

			Assert.AreEqual(1, singleWeakestPlanet.Count);
			Assert.AreEqual(3, singleWeakestPlanet[0].PlanetID());
		}

		[TestMethod]
		public void TestSomeWeakestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets singleWeakestPlanet = bot.WeakestPlanets(bot.Context.Planets(), 3);

			Assert.AreEqual(3, singleWeakestPlanet.Count);
			Assert.IsTrue((singleWeakestPlanet[0].PlanetID() == 3) || (singleWeakestPlanet[0].PlanetID() == 4));
			Assert.IsTrue((singleWeakestPlanet[1].PlanetID() == 3) || (singleWeakestPlanet[1].PlanetID() == 4));
			Assert.IsTrue((singleWeakestPlanet[2].PlanetID() == 1) || (singleWeakestPlanet[2].PlanetID() == 2));
			
		}

		[TestMethod]
		public void TestSingleStrongestPlanet()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			List<Planet> singleStrongestPlanet = bot.StrongestPlanets(bot.Context.Planets(), 1);

			Assert.AreEqual(1, singleStrongestPlanet.Count);
			Assert.AreEqual(0, singleStrongestPlanet[0].PlanetID());
		}

		[TestMethod]
		public void TestSomeStrongestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets strongestPlanets = bot.StrongestPlanets(bot.Context.Planets(), 3);

			Assert.AreEqual(3, strongestPlanets.Count);
			Assert.AreEqual(0, strongestPlanets[0].PlanetID());
			Assert.IsTrue((strongestPlanets[1].PlanetID() == 1) || (strongestPlanets[1].PlanetID() == 2));
			Assert.IsTrue((strongestPlanets[2].PlanetID() == 1) || (strongestPlanets[2].PlanetID() == 2));
		}

		[TestMethod]
		public void TestMyWeakestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets weakestPlanets = bot.MyWeakestPlanets(2);

			Assert.AreEqual(2, weakestPlanets.Count);
			Assert.AreEqual(3, weakestPlanets[0].PlanetID());
			Assert.AreEqual(1, weakestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestMyStrongestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets strongestPlanets = bot.MyStrongestPlanets(2);

			Assert.AreEqual(2, strongestPlanets.Count);
			Assert.AreEqual(1, strongestPlanets[0].PlanetID());
			Assert.AreEqual(3, strongestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestNeutralWeakestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets weakestPlanets = bot.NeutralWeakestPlanets(2);

			Assert.AreEqual(2, weakestPlanets.Count);
			Assert.AreEqual(4, weakestPlanets[0].PlanetID());
			Assert.AreEqual(0, weakestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestNeutralStrongestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets strongestPlanets = bot.NeutralStrongestPlanets(3);

			Assert.AreEqual(2, strongestPlanets.Count);
			Assert.AreEqual(0, strongestPlanets[0].PlanetID());
			Assert.AreEqual(4, strongestPlanets[1].PlanetID());
		}

		[TestMethod]
		public void TestEnemyWeakestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets weakestPlanets = bot.EnemyWeakestPlanets(2);

			Assert.AreEqual(1, weakestPlanets.Count);
			Assert.AreEqual(2, weakestPlanets[0].PlanetID());
		}

		[TestMethod]
		public void TestEnemyStrongestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets strongestPlanets = bot.EnemyStrongestPlanets(3);

			Assert.AreEqual(1, strongestPlanets.Count);
			Assert.AreEqual(2, strongestPlanets[0].PlanetID());
		}

		[TestMethod]
		public void TestPlanetsWithGivenOwner()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets neutralPlanets = bot.PlanetsWithGivenOwner(bot.Context.Planets(), 0);

			Assert.AreEqual(2, neutralPlanets.Count);
			Assert.IsTrue((neutralPlanets[0].PlanetID() == 0) || (neutralPlanets[0].PlanetID() == 4));
			Assert.IsTrue((neutralPlanets[1].PlanetID() == 0) || (neutralPlanets[1].PlanetID() == 4));
		}

		[TestMethod]
		public void TestFleetsWithGivenOwner()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Fleets myFleets = bot.FleetsWithGivenOwner(bot.Context.Fleets(), 1);

			Assert.AreEqual(2, myFleets.Count);
			Assert.AreEqual(75, myFleets[0].NumShips() + myFleets[1].NumShips());
		}

		[TestMethod]
		public void TestFleetsGoingToPlanet()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Fleets attackingFleets = bot.FleetsGoingToPlanet(bot.Context.Fleets(), bot.Context.GetPlanet(4));

			Assert.AreEqual(2, attackingFleets.Count);
			Assert.AreEqual(55, attackingFleets[0].NumShips() + attackingFleets[1].NumShips());

			attackingFleets = bot.MyFleetsGoingToPlanet(bot.Context.GetPlanet(4));

			Assert.AreEqual(1, attackingFleets.Count);
			Assert.AreEqual(25, attackingFleets[0].NumShips());

			attackingFleets = bot.EnemyFleetsGoingToPlanet(bot.Context.GetPlanet(4));

			Assert.AreEqual(1, attackingFleets.Count);
			Assert.AreEqual(30, attackingFleets[0].NumShips());
		}

		[TestMethod]
		public void TestPlanetsUnderAttack()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets planets = bot.MyPlanetsUnderAttack();

			Assert.AreEqual(1, planets.Count);
			Assert.AreEqual(1, planets[0].PlanetID());

			planets = bot.NeutralPlanetsUnderAttack();

			Assert.AreEqual(1, planets.Count);
			Assert.AreEqual(4, planets[0].PlanetID());
		}

		[TestMethod]
		public void TestPlanetsWithinProximityToPlanet()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets nearPlanets = bot.PlanetsWithinProximityToPlanet(bot.Context.Planets(), bot.Context.GetPlanet(1), 11);

			Assert.AreEqual(2, nearPlanets.Count);
			Assert.IsTrue((nearPlanets[0].PlanetID() == 0) || (nearPlanets[0].PlanetID() == 3));
			Assert.IsTrue((nearPlanets[1].PlanetID() == 0) || (nearPlanets[1].PlanetID() == 3));
		}

		[TestMethod]
		public void TestPlanetFutureStatus()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planet futurePlanet = bot.PlanetFutureStatus(bot.Context.GetPlanet(4), 3);

			Assert.AreEqual(12, futurePlanet.NumShips());
			Assert.AreEqual(1, futurePlanet.Owner());

			futurePlanet = bot.PlanetFutureStatus(bot.Context.GetPlanet(4), 2);

			Assert.AreEqual(9, futurePlanet.NumShips());
			Assert.AreEqual(2, futurePlanet.Owner());
		}

		[TestMethod]
		public void TestMyEndangeredPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			Planets planets = bot.MyEndangeredPlanets(1, 10);

			Assert.AreEqual(1, planets.Count);
			Assert.AreEqual(1, planets[0].PlanetID());
		}
	}
}
