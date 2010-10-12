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
		public void TestCombinations()
		{
			const string message =
				"P 0 0 1 100 0#0\n" +
				"P 2 2 2 10 5#1\n" +
				"P 3 3 0 30 3#2  0.6\n" +
				"P 4 4 0 50 5#3  0.83\n" +
				"P 5 5 0 40 4#4  0.5\n" +
				"P 6 6 2 10 4#5\n";

			PlanetWars pw = new PlanetWars(message);
			FirstMoveAdviser adviser = new FirstMoveAdviser(pw);
			Planets targetPlanets = adviser.Knapsack01(pw.NeutralPlanets(), 100);
				//FirstMoveAdviser.GetTargetPlanets(pw.NeutralPlanets(), 100);

			Assert.AreEqual(2, targetPlanets.Count);
			Assert.IsTrue(targetPlanets[0].PlanetID() == 2 || targetPlanets[1].PlanetID() == 2);
			Assert.IsTrue(targetPlanets[0].PlanetID() == 3 || targetPlanets[1].PlanetID() == 3);
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

		/*[TestMethod]
		public void TestClosePosition()
		{
			const string message =
				"P 10.387292 9.441675 0 57 2#0\n" +
				"P 8.703419 10.893037 1 100 5#1\n" +
				"P 12.071165 7.990313 2 100 5#2\n" +
				"P 16.190119 1.0816476 0 70 1#3\n" +
				"P 4.584465 17.801702 0 70 1#4\n" +
				"P 5.275277 14.12433 0 66 3#5\n" +
				"P 15.499307 4.7590203 0 66 3#6\n" +
				"P 3.50396 14.385995 0 52 4#7\n" +
				"P 17.270624 4.4973555 0 52 4#8\n" +
				"P 19.870605 0 0 50 3#9\n" +
				"P 0.9039775 18.88335 0 50 3#10\n" +
				"P 1.7237648 14.834088 0 65 2#11\n" +
				"P 19.05082 4.0492616 0 65 2#12\n" +
				"P 19.01008 13.580164 0 68 3#13\n" +
				"P 1.7645035 5.3031864 0 68 3#14\n" +
				"P 1.568742 3.5232484 0 84 2#15\n" +
				"P 19.205841 15.360102 0 84 2#16\n" +
				"P 20.774584 17.988132 0 20 5#17\n" +
				"P 0 0.8952183 0 20 5#18\n" +
				"P 8.149417 7.9227724 0 90 2#19\n" +
				"P 12.625167 10.960578 0 90 2#20\n" +
				"P 0.29697508 16.879242 0 63 5#21\n" +
				"P 20.477608 2.0041084 0 63 5#22\n" +
				"go\n";

			PlanetWars pw = new PlanetWars(message);
			FirstMoveAdviser adviser = new FirstMoveAdviser(pw);
			List<MovesSet> movesSet = adviser.RunAll();

			Assert.IsTrue(movesSet.Count > 0);
		}*/
	}
}
