#define DEBUG

using System.Globalization;
using System.Threading;
using Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Fleets = System.Collections.Generic.List<Bot.Fleet>;

namespace BotTests
{
	/// <summary>
	/// Summary description for SupplyAdviserTests
	/// </summary>
	[TestClass]
	public class SupplyAdviserTests
	{
		[TestInitialize]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestSupplyFrontPlanet()
		{
			const string message =
				"P 2 2 1 10 0#0\n" +
				"P 4 1 1 10 5#1\n" +
				"P 5 6 1 10 5#2\n" +
				"P 0 0 1 10 4#3\n" +
				"P 2 5 2 10 4#4\n" +
				"P 3 3 2 10 4#5\n";
			PlanetWars pw = new PlanetWars(message);
			Config.InvokeDistanceForFront = 3;

			IAdviser adviser = new SupplyAdviser(pw, pw.GetPlanet(3));
			Moves moves = adviser.Run();

			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual(3, moves[0].SourceID);
			Assert.AreEqual(0, moves[0].DestinationID);
			Assert.AreEqual(10, moves[0].NumSheeps);
		}

		[TestMethod]
		public void TestSupplyFrontPlanet2()
		{
			const string message =
				"P 13.3422077014 0.781586578168 1 100 5\n" +
				"P 9.83220591213 22.0204604189 2 100 5\n" +
				"P 19.6930466168 9.76544103518 2 2 4\n" +
				"P 20.2820697829 3.88811462784 1 7 1\n";
			Config.InvokeDistanceForFront = 10;
			PlanetWars pw = new PlanetWars(message);

			IAdviser adviser = new SupplyAdviser(pw, pw.GetPlanet(0));
			Moves moves = adviser.Run();

			Assert.AreEqual(1, moves.Count);
			/*Assert.AreEqual(3, moves[0].SourceID);
			Assert.AreEqual(0, moves[0].DestinationID);
			Assert.AreEqual(10, moves[0].NumSheeps);*/
		}

		[TestMethod]
		public void TestDontInvadeWhenInDanger()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.6135908004 11.6587374197 0 119 0\n" +
				"P 1.2902863101 9.04078582767 1 0 5\n" +
				"P 21.9368952907 14.2766890117 2 88 5\n" +
				"P 5.64835767563 18.2659924733 1 0 4\n" +
				"P 17.5788239251 5.05148236609 0 21 4\n" +
				"P 0.0 17.5664628114 1 39 2\n" +
				"P 23.2271816008 5.75101202793 1 2 2\n" +
				"P 15.9964071303 22.4925373322 1 17 5\n" +
				"P 7.23077447046 0.824937507164 0 60 5\n" +
				"P 12.096860926 23.3174748393 0 74 5\n" +
				"P 11.1303206747 0.0 0 74 5\n" +
				"P 5.90572926007 2.48227346488 0 85 1\n" +
				"P 17.3214523407 20.8352013745 0 85 1\n" +
				"P 18.2860133478 0.765777669475 0 72 3\n" +
				"P 4.94116825299 22.5516971699 0 68 3\n" +
				"P 20.1067105381 18.0593851211 2 113 5\n" +
				"P 3.12047106262 5.25808971821 1 8 5\n" +
				"P 4.594838746 13.7860000656 0 69 2\n" +
				"P 18.6323428548 9.5314747737 1 1 2\n" +
				"P 8.80119206169 20.0157034284 1 5 1\n" +
				"P 14.4259895391 3.30177141098 2 18 1\n" +
				"P 19.4667873213 20.0561682576 1 195 5\n" +
				"P 3.76039427948 3.26130658173 0 35 5\n" +
				"F 1 6 19 20 18 1\n" +
				"F 1 24 21 20 18 1\n" +
				"F 1 8 3 20 18 1\n" +
				"F 2 50 2 5 23 6\n" +
				"F 2 25 20 5 21 5\n" +
				"F 1 12 7 5 17 6\n" +
				"F 1 69 21 14 15 4\n" +
				"F 2 57 15 16 22 11\n" +
				"F 1 10 16 3 14 4\n" +
				"F 1 10 16 3 14 5\n" +
				"F 1 10 16 3 14 6\n" +
				"F 2 48 2 1 22 14\n" +
				"F 1 6 16 3 14 7\n" +
				"F 1 12 19 7 8 1\n" +
				"F 1 5 16 3 14 8\n" +
				"F 1 10 19 7 8 2\n" +
				"F 1 5 16 3 14 9\n" +
				"F 1 10 19 7 8 3\n" +
				"F 1 1 6 21 15 11\n" +
				"F 1 5 16 3 14 10\n" +
				"F 1 10 19 7 8 4\n" +
				"F 1 17 7 21 5 1\n" +
				"F 1 2 6 21 15 12\n" +
				"F 1 5 16 3 14 11\n" +
				"F 1 10 19 7 8 5\n" +
				"F 1 4 3 19 4 1\n" +
				"F 1 5 7 21 5 2\n" +
				"F 1 2 6 21 15 13\n" +
				"F 1 10 16 3 14 12\n" +
				"F 1 5 19 7 8 6\n" +
				"F 1 4 3 19 4 2\n" +
				"F 1 22 7 21 5 3\n" +
				"F 1 2 6 21 15 14\n" +
				"F 1 10 16 3 14 13\n" +
				"F 1 13 19 7 8 7\n" +
				"F 1 9 3 19 4 3\n" +
				"F 1 22 7 21 5 4\n" +
				"F 1 3 5 4 6 6\n" +
				"go\n");

			SupplyAdviser adviser = new SupplyAdviser(planetWars);
			Planets myPlanets = planetWars.MyPlanets();
			foreach (Planet planet in myPlanets)
			{
				adviser.SupplyPlanet = planet;
				Moves moves = adviser.Run();
				foreach (Move move in moves)
				{
					Assert.IsFalse(move.SourceID == 5);
					planetWars.IssueOrder(move);
				}
			}
		}
	}
}
