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
		public void TestCompareDistanceToTargetPlanet()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 1 1 0 119 0#0\n" +
				"P 5 5 1 40 5#1\n" +
				"P 2 2 1 100 5#2\n" +
				"P 4 4 1 100 5#3\n" +
				"go\n");

			Comparer comparer = new Comparer(planetWars);
			comparer.TargetPlanet = planetWars.GetPlanet(0);
			Planets planets = planetWars.MyPlanets();
			planets.Sort(comparer.CompareDistanceToTargetPlanetLT);

			Assert.AreEqual(2, planets[0].PlanetID());
		}
	}
}
