using System.Globalization;
using System.Threading;
using Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace BotTests
{
	/// <summary>
	/// Summary description for InvadeAdviserTests
	/// </summary>
	[TestClass]
	public class InvadeAdviserTests
	{
		// Use TestInitialize to run code before running each test 
		[TestInitialize]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestDoNothingWhenNoNeutralPlanets()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.6135908004 11.6587374197 1 119 0#0\n" +
				"P 1.2902863101 9.04078582767 1 40 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 2.64835767563 10.2659924733 1 21 4#3\n" +
				"P 17.5788239251 5.05148236609 2 21 4#4\n" +
				"go\n");

			IAdviser adviser = new InvadeAdviser(planetWars);
			Moves moves = adviser.Run();

			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestInvadeNeutralPlanet()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 1 1 0 119 0#0\n" +
				"P 2 2 1 40 5#1\n" +
				"P 3 3 2 100 5#2\n" +
				"P 4 4 1 21 4#3\n" +
				"P 5 5 0 21 0#4\n" +
				"go\n");
			
			IAdviser adviser = new InvadeAdviser(planetWars);
			Moves moves = adviser.Run();

			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual(4, moves[0].DestinationID);
			Assert.AreEqual(21 + Config.MinShipsOnPlanetsAfterInvade, moves[0].NumSheeps);
		}
	}
}
