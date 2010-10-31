using System;
using System.Collections.Generic;
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
			List<MovesSet> moves = adviser.RunAll();

			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestInvadeNeutralPlanet()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 1 1 0 119 2#0\n" +
				"P 2 2 1 150 5#1\n" +
				"P 3 3 2 100 5#2\n" +
				"P 6 6 1 21 2#3\n" +
				"P 5 5 0 21 5#4\n" +
				"go\n");

			Config.InvadeSendMoreThanEnemyCanDefend = true;
			
			IAdviser adviser = new InvadeAdviser(planetWars);
			Moves moves = adviser.Run(planetWars.GetPlanet(4));

			int extraTurns = (int)Math.Ceiling(planetWars.GetPlanet(4).NumShips() / (double)planetWars.GetPlanet(4).GrowthRate());

			Assert.AreEqual(2, moves.Count);
			Assert.AreEqual(21 + 100 + 5 * 2 + 1, moves[0].NumSheeps + moves[1].NumSheeps);
		}

		[TestMethod]
		public void TestNeverInvadePlanetWithZeroGrowLevel()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 1 1 1 100 5#0\n" +
				"P 2 2 0 1 0#1\n" +
				"go\n");

			IAdviser adviser = new InvadeAdviser(planetWars);
			List<MovesSet> moves = adviser.RunAll();

			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestInvadeCorrectNumber2()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 10.946215 11.757139 0 15 4#0\n" +
				"P 18.033072 20.11299 1 6 5#1\n" +
				"P 3.859357 3.4012883 2 130 5#2\n" +
				"P 19.282667 9.50695 0 61 2#3\n" +
				"P 2.609762 14.007327 0 61 2#4\n" +
				"P 4.9880333 6.555844 0 15 2#5\n" +
				"P 16.904396 16.958433 1 5 2#6\n" +
				"P 17.819895 5.624568 0 79 3#7\n" +
				"P 4.0725336 17.88971 0 79 3#8\n" +
				"P 12.936233 0.2987193 0 36 1#9\n" +
				"P 8.956196 23.215559 0 36 1#10\n" +
				"P 0 23.514278 0 43 1#11\n" +
				"P 21.89243 0 0 43 1#12\n" +
				"P 13.72995 13.885786 0 59 2#13\n" +
				"P 8.162479 9.628491 0 59 2#14\n" +
				"P 7.9460754 4.9982805 0 87 2#15\n" +
				"P 13.946353 18.515997 0 87 2#16\n" +
				"P 5.118788 4.420579 0 26 1#17\n" +
				"P 16.773642 19.093699 1 6 1#18\n" +
				"P 17.77824 11.674772 0 62 1#19\n" +
				"P 4.1141887 11.839505 0 62 1#20\n" +
				"P 20.811337 21.04995 1 5 5#21\n" +
				"P 1.0810924 2.4643273 0 38 5#22\n" +
				"F 1 5 21 1 3 1\n" +
				"F 1 15 18 0 10 8\n" +
				"F 1 5 1 18 2 1\n" +
				"F 1 5 21 1 3 2\n" +
				"F 1 22 18 6 3 2\n" +
				"go\n"
				);

			IAdviser adviser = new InvadeAdviser(planetWars);
			List<MovesSet> movesList = adviser.RunAll();

			bool sended = false;
			foreach (MovesSet movesSet in movesList)
			{
				Moves moves = movesSet.GetMoves();
				foreach (Move move in moves)
				{
					if (move.DestinationID == 0)
					{
						sended = true;
						Assert.AreEqual(1, move.NumSheeps);
						Assert.AreEqual(0, move.TurnsBefore);
					}
				}
			}
			Assert.IsTrue(sended);
		}

	}
}
