using System;
using System.Collections.Generic;
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
	/// Summary description for StealAdviserTests
	/// </summary>
	[TestClass]
	public class FirstMoveAdviserTests
	{
		[TestInitialize]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestDoSomething()
		{
			const string message =
				"P 10.621908 8.30391 0 129 0#0\n" +
				"P 10.514905 1.7481457 1 100 5#1\n" +
				"P 10.728911 14.859674 2 100 5#2\n" +
				"P 13.951591 15.502133 0 88 2#3\n" +
				"P 7.292225 1.1056871 0 88 2#4\n" +
				"P 2.2078469 13.372326 0 87 4#5\n" +
				"P 19.035969 3.235495 0 87 4#6\n" +
				"P 5.829895 0 0 90 1#7\n" +
				"P 15.41392 16.60782 0 90 1#8\n" +
				"P 13.174852 11.510555 0 1 4#9\n" +
				"P 8.068963 5.0972652 0 1 4#10\n" +
				"P 21.243816 1.3553712 0 8 2#11\n" +
				"P 0 15.252449 0 8 2#12\n" +
				"P 16.312483 1.6560206 0 8 1#13\n" +
				"P 4.9313326 14.9518 0 8 1#14\n" +
				"P 7.9964867 7.850896 0 24 3#15\n" +
				"P 13.247329 8.756925 0 24 3#16\n" +
				"P 19.656878 1.6819931 0 3 2#17\n" +
				"P 1.5869379 14.925827 0 3 2#18\n" +
				"P 9.85971 10.874075 0 76 2#19\n" +
				"P 11.384106 5.7337456 0 76 2#20\n" +
				"P 2.0674944 1.7542361 0 80 5#21\n" +
				"P 19.176321 14.853584 0 80 5#22\n" +
				"go\n";

			PlanetWars pw = new PlanetWars(message);
			FirstMoveAdviser adviser = new FirstMoveAdviser(pw);
			List<MovesSet> movesSet = adviser.RunAll();

			Assert.IsTrue(movesSet.Count > 0);
		}

		[TestMethod]
		public void TestDontGoToPlanetsCloserToEnemy()
		{
			const string message =
				"P 0 0 1 57 2#0\n" +
				"P 6 6 2 100 5#1\n" +
				"P 2 2 0 1 5#2\n" +
				"P 4 4 0 1 100#3\n" +
				"go\n";

			PlanetWars pw = new PlanetWars(message);
			FirstMoveAdviser adviser = new FirstMoveAdviser(pw);
			List<MovesSet> movesSet = adviser.RunAll();

			foreach (MovesSet set in movesSet)
			{
				Moves moves = set.GetMoves();
				foreach (Move move in moves)
				{
					Assert.IsFalse(move.DestinationID == 3);
				}
			}
		}

		[TestMethod]
		public void TestReturners()
		{
			const string message =
				"P 0 0 1 100 5#0\n" +
				"P 9 0 2 100 5#1\n" +
				"P 2 0 0 59 5#2\n" +
				"go\n";

			PlanetWars pw = new PlanetWars(message);
			FirstMoveAdviser adviser = new FirstMoveAdviser(pw);
			List<MovesSet> movesSet = adviser.RunAll();

			Assert.AreEqual(1, movesSet.Count);
			Assert.AreEqual(1, movesSet[0].GetMoves().Count);
			Assert.AreEqual(60, movesSet[0].GetMoves()[0].NumShips);
		}

		[TestMethod]
		public void TestNoOverShips()
		{
			const string message =
				"P 9.790385 11.939511 0 3 4#0\n" +
				"P 13.135618 2.07503 1 100 5#1\n" +
				"P 6.4451513 21.803991 2 100 5#2\n" +
				"P 0 3.3089578 0 58 1#3\n" +
				"P 19.58077 20.570065 0 58 1#4\n" +
				"P 11.975901 0.2549936 0 69 1#5\n" +
				"P 7.604869 23.624027 0 69 1#6\n" +
				"P 4.7522573 6.9940777 0 48 1#7\n" +
				"P 14.828512 16.884945 0 48 1#8\n" +
				"P 14.284102 12.164202 0 82 2#9\n" +
				"P 5.296667 11.71482 0 82 2#10\n" +
				"P 18.723335 9.709133 0 6 4#11\n" +
				"P 0.8574339 14.1698885 0 6 4#12\n" +
				"P 18.553186 5.650815 0 17 4#13\n" +
				"P 1.0275841 18.228207 0 17 4#14\n" +
				"P 3.260637 6.3643866 0 47 2#15\n" +
				"P 16.320133 17.514635 0 47 2#16\n" +
				"P 6.901025 0 0 49 2#17\n" +
				"P 12.679745 23.879023 0 49 2#18\n" +
				"P 18.747196 2.6167994 0 28 3#19\n" +
				"P 0.8335743 21.262222 0 28 3#20\n" +
				"P 5.718854 1.5311116 0 11 3#21\n" +
				"P 13.861916 22.34791 0 11 3#22\n" +
				"go\n";

			PlanetWars pw = new PlanetWars(message);
			FirstMoveAdviser adviser = new FirstMoveAdviser(pw);
			List<MovesSet> movesSet = adviser.RunAll();

			Assert.AreEqual(1, movesSet.Count);
			int ships = 0;
			foreach (Move move in movesSet[0].GetMoves())
			{
				ships += move.NumShips;
			}
			Console.WriteLine(ships);
			Assert.IsTrue(100 >= ships);
		}

		[TestMethod]
		public void Test2Pow()
		{
			for (int i = 0; i < 10; i++)
			{
				Console.WriteLine(1 << i);
			}
		}

		[TestMethod]
		public void TestClosePosition()
		{
			const string message =
				"P 14 14 0 35 5#0\n" +
				"P 7.2951994 8.103587 1 100 5#1\n" +
				"P 5.5505366 11.114068 2 100 5#2\n" +
				"P 4.8750625 8.711828 0 86 4#3\n" +
				"P 7.181268 10.048343 0 68 3#4\n" +
				"P 8.658058 13.022591 0 15 3#5\n" +
				"P 10.4961 9.850982 0 15 3#6\n" +
				"P 17.795769 17.857422 0 79 5#7\n" +
				"P 19.234045 15.375622 0 79 5#8\n" +
				"P 17.824682 18.873695 0 84 3#9\n" +
				"P 20.130194 14.89545 0 84 3#10\n" +
				"P 20.19213 8.496058 0 20 2#11\n" +
				"P 12.30302 22.109013 0 20 2#12\n" +
				"P 5.0697794 15.698908 0 31 2#13\n" +
				"P 11.034236 5.4070163 0 31 2#14\n" +
				"P 9.207518 9.434581 0 35 3#15\n" +
				"P 7.6561236 12.111569 0 35 3#16\n" +
				"P 5.7288275 13.711043 0 27 5#17\n" +
				"P 9.6371155 6.967146 0 27 5#18\n" +
				"go\n";

			PlanetWars pw = new PlanetWars(message);
			FirstMoveAdviser adviser = new FirstMoveAdviser(pw);
			List<MovesSet> movesSet = adviser.RunAll();

			Assert.IsTrue(movesSet.Count == 1);
			//go to 27 planet with 20 ships, to protect from RageBot and take bigger planet than 15
			Assert.AreEqual(20, movesSet[0].SummaryNumShips);
		}
	}
}
