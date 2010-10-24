using System.Globalization;
using System.Threading;
using Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace BotTests
{
	/// <summary>
	/// Summary description for DijkstraPathFinderTests
	/// </summary>
	[TestClass]
	public class DijkstraPathFinderTests
	{
		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestShortestPath()
		{
			PlanetWars planetWars = new PlanetWars(
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

			Planets frontPlanets = planetWars.GetFrontPlanets();
			Assert.AreEqual(2, frontPlanets.Count);
			Assert.AreEqual(5 + 7, frontPlanets[0].PlanetID() + frontPlanets[1].PlanetID());

			DijkstraPathFinder pf = new DijkstraPathFinder(planetWars);
			Planet nextPlanet = pf.FindNextPlanetInPath(planetWars.GetPlanet(0));

			Assert.AreEqual(2, nextPlanet.PlanetID());
		}
	}
}
