using System.Globalization;
using System.Threading;
using Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace BotTests
{
	/// <summary>
	/// Summary description for ComparerTests
	/// </summary>
	[TestClass]
	public class ComparerTests
	{
		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestCompareImportance()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.6135908004 11.6587374197 0 119 0#0\n" +
				"P 1.2902863101 9.04078582767 1 40 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 2.64835767563 10.2659924733 1 21 4#3\n" +
				"P 11.6135908004 11.6587374197 0 21 0#4\n" +
				"go\n");

			Comparer comparer = new Comparer(planetWars);
			Planets planets = planetWars.NeutralPlanets();
			planets.Sort(comparer.CompareImportanceOfPlanetsGT);

			Assert.AreEqual(4, planets[0].PlanetID());
		}

		[TestMethod]
		public void TestCompareDistanceToTargetPlanet()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 1 1 0 119 0#0\n" +
				"P 5 5 1 40 5#1\n" +
				"P 2 2 1 100 5#2\n" +
				"go\n");

			Comparer comparer = new Comparer(planetWars);
			Planets planets = planetWars.MyPlanets();
			planets.Sort(comparer.CompareDistanceToTargetPlanetGT);

			Assert.AreEqual(2, planets[0].PlanetID());
		}
	}
}
