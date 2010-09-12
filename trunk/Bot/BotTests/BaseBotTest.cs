using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bot;

namespace BotTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class BaseBotTests
	{
		public BaseBotTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		// Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

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
		// [TestInitialize()]
		// public void MyTestInitialize() { }
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
				"P 1.2902863101 9.04078582767 1 100 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 5.64835767563 18.2659924733 0 21 4#3\n" +
				"P 17.5788239251 5.05148236609 0 21 4#4\n" +
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
			List<Planet> singleWeakestPlanet = bot.WeakestPlanets(bot.Context.Planets(), 1);

			Assert.AreEqual(1, singleWeakestPlanet.Count);
			Assert.AreEqual(3, singleWeakestPlanet[0].PlanetID());
		}

		[TestMethod]
		public void TestSomeWeakestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			List<Planet> singleWeakestPlanet = bot.WeakestPlanets(bot.Context.Planets(), 3);

			Assert.AreEqual(3, singleWeakestPlanet.Count);
			Assert.IsTrue((singleWeakestPlanet[0].PlanetID() == 3) || (singleWeakestPlanet[0].PlanetID() == 4));
			Assert.IsTrue((singleWeakestPlanet[1].PlanetID() == 3) || (singleWeakestPlanet[1].PlanetID() == 4));
			Assert.IsTrue((singleWeakestPlanet[2].PlanetID() == 1) || (singleWeakestPlanet[2].PlanetID() == 2));
			
		}

		[TestMethod]
		public void TestSingleStrongestPlanet()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			List<Planet> singleWeakestPlanet = bot.StrongestPlanets(bot.Context.Planets(), 1);

			Assert.AreEqual(1, singleWeakestPlanet.Count);
			Assert.AreEqual(0, singleWeakestPlanet[0].PlanetID());
		}

		[TestMethod]
		public void TestSomeStrongestPlanets()
		{
			BaseBot bot = new BaseBot(CreateTestContextForSort());
			List<Planet> singleWeakestPlanet = bot.WeakestPlanets(bot.Context.Planets(), 3);

			Assert.AreEqual(3, singleWeakestPlanet.Count);
			Assert.AreEqual(0, singleWeakestPlanet[0].PlanetID());
			Assert.IsTrue((singleWeakestPlanet[1].PlanetID() == 1) || (singleWeakestPlanet[1].PlanetID() == 2));
			Assert.IsTrue((singleWeakestPlanet[2].PlanetID() == 1) || (singleWeakestPlanet[2].PlanetID() == 2));

		}
	}
}
