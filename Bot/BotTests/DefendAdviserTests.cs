using System.Globalization;
using System.Threading;
using Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace BotTests
{
	/// <summary>
	/// Summary description for DefendAdviserTests
	/// </summary>
	[TestClass]
	public class DefendAdviserTests
	{
		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestDoNothingWhenNobodyAttacks()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.6135908004 11.6587374197 0 119 0#0\n" +
				"P 1.2902863101 9.04078582767 1 40 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 2.64835767563 10.2659924733 1 21 4#3\n" +
				"P 17.5788239251 5.05148236609 0 21 4#4\n" +
				"F 1 25 1 4 5 3\n" +
				"F 1 50 1 2 10 1\n" +
				"F 2 30 2 4 5 2\n" +
				"go\n");

			IAdviser adviser = new DefendAdviser(planetWars);
			Moves moves = adviser.Run();

			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestDefendPlanetUnderAttack()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.6135908004 11.6587374197 0 119 0#0\n" +
				"P 1.2902863101 9.04078582767 1 40 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 2.64835767563 10.2659924733 1 31 4#3\n" +
				"P 17.5788239251 5.05148236609 0 21 4#4\n" +
				"F 1 25 1 4 5 3\n" +
				"F 1 50 1 2 10 1\n" +
				"F 2 70 2 1 5 3\n" +
				"go\n");

			IAdviser adviser = new DefendAdviser(planetWars);
			Moves moves = adviser.Run();

			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual(3, moves[0].SourceID);
			Assert.AreEqual(1, moves[0].DestinationID);
			Assert.AreEqual(15, moves[0].NumSheeps);
		}
	}
}
