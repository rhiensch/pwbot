using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moves = System.Collections.Generic.List<Bot.Move>;


namespace BotTests
{
	/// <summary>
	/// Summary description for AttackAdviserTests
	/// </summary>
	[TestClass]
	public class AttackAdviserTests
	{
		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestDoNothingWhenNoSuitablePlanets()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.6135908004 11.6587374197 0 119 0#0\n" +
				"P 1.2902863101 9.04078582767 1 40 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 2.64835767563 10.2659924733 1 21 4#3\n" +
				"P 17.5788239251 5.05148236609 0 21 4#4\n" +
				"go\n");

			IAdviser adviser = new AttackAdviser(planetWars);
			List<MovesSet> moves = adviser.RunAll();

			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestAttack()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 10.6545618495 10.9391936067 1 9 2\n" +
				"P 10.619331099 20.0028830106 1 5 5\n"+
				"P 10.6897926 1.87550420275 2 74 5\n"+
				"P 18.53031859 17.0517193434 0 80 5\n"+
				"P 2.77880510898 4.82666786996 0 80 5\n"+
				"P 21.309123699 21.8783872134 1 42 2\n"+
				"P 0 0 0 27 2\n"+
				"P 1.87887471354 12.4081640973 0 31 1\n"+
				"P 2.20229281225 21.5740106508 0 39 1\n"+
				"P 19.1068308868 0.304376562584 1 1 1\n"+
				"P 6.42926362386 21.4118201349 1 72 4\n"+
				"P 14.8798600751 0.46656707848 1 77 4\n"+
				"P 1.00463725255 15.9311569071 1 40 4\n"+
				"P 20.3044864465 5.94723030628 0 9 4\n"+
				"P 14.2779222572 9.66512575619 0 77 2\n"+
				"P 7.03120144184 12.2132614572 0 77 2\n"+
				"P 14.504667316 11.7232002564 0 80 4\n"+
				"P 6.80445638303 10.155186957 0 80 4\n"+
				"P 8.64518320868 11.6016961635 0 69 2\n"+
				"P 12.6639404903 10.2766910498 0 69 2\n"+
				"P 18.8599135545 21.6009176881 0 85 1\n"+
				"P 2.44921014454 0.277469525237 0 85 1\n"+
				"go\n");

			IAdviser adviser = new AttackAdviser(planetWars);
			List<MovesSet> moves = adviser.RunAll();

			Assert.IsTrue(moves.Count > 0);
		}

		[TestMethod]
		public void TestDontAttackWhenPlanetIsStronger()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 0 0 1 10 5#0\n" +
				"P 5 5 1 10 5#1\n" +
				"P 2 2 2 2 5#2\n" +
				"P 3 3 2 2 5#3\n" +
				"P 4 4 2 10 5#4\n" +
				"go\n");

			AttackAdviser adviser = new AttackAdviser(planetWars);
			Moves moves = adviser.Run(planetWars.GetPlanet(4));

			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestAttackWhenOurFirstPlanetStrongerThanDefenders()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 0 0 1 10 5#0\n" +
				"P 5 5 1 100 5#1\n" +
				"P 2 2 2 2 5#2\n" +
				"P 3 3 2 2 5#3\n" +
				"P 4 4 2 10 5#4\n" +
				"go\n");

			AttackAdviser adviser = new AttackAdviser(planetWars);
			Moves moves = adviser.Run(planetWars.GetPlanet(4));

			Assert.AreEqual(1, moves.Count);
			Assert.AreEqual(100, moves[0].NumSheeps);
		}

		[TestMethod]
		public void TestAttackWhenMyAllPlanetsStrongerThanDefenders()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 0 0 1 100 5#0\n" +
				"P 5 5 1 10 5#1\n" +
				"P 2 2 2 2 5#2\n" +
				"P 3 3 2 2 5#3\n" +
				"P 4 4 2 10 5#4\n" +
				"go\n");

			AttackAdviser adviser = new AttackAdviser(planetWars);
			Moves moves = adviser.Run(planetWars.GetPlanet(4));

			Assert.AreEqual(2, moves.Count);
			Assert.AreEqual(120, moves[0].NumSheeps + moves[1].NumSheeps);
		}

		[TestMethod]
		public void TestDoAttackWhenPossible()
		{
			PlanetWars pw = new PlanetWars(
				"P 9.790385 11.939511 1 23 4#0\n" +
				"P 13.135618 2.07503 1 5 5#1\n" +
				"P 6.4451513 21.803991 2 320 5#2\n" +
				"P 0 3.3089578 0 58 1#3\n" +
				"P 19.58077 20.570065 0 58 1#4\n" +
				"P 11.975901 0.2549936 0 69 1#5\n" +
				"P 7.604869 23.624027 0 69 1#6\n" +
				"P 4.7522573 6.9940777 0 48 1#7\n" +
				"P 14.828512 16.884945 0 48 1#8\n" +
				"P 14.284102 12.164202 0 82 2#9\n" +
				"P 5.296667 11.71482 0 82 2#10\n" +
				"P 18.723335 9.709133 1 16 4#11\n" +
				"P 0.8574339 14.1698885 1 27 4#12\n" +
				"P 18.553186 5.650815 1 7 4#13\n" +
				"P 1.0275841 18.228207 1 357 4#14\n" +
				"P 3.260637 6.3643866 0 47 2#15\n" +
				"P 16.320133 17.514635 0 47 2#16\n" +
				"P 6.901025 0 0 49 2#17\n" +
				"P 12.679745 23.879023 0 49 2#18\n" +
				"P 18.747196 2.6167994 1 3 3#19\n" +
				"P 0.8335743 21.262222 0 28 3#20\n" +
				"P 5.718854 1.5311116 1 3 3#21\n" +
				"P 13.861916 22.34791 1 3 3#22\n" +
				"go\n");

			AttackAdviser adviser = new AttackAdviser(pw);
			List<MovesSet> moves = adviser.RunAll();

			Assert.IsTrue(moves.Count > 0);
			Assert.AreEqual(357, moves[0].Moves[0].NumSheeps);
		}
	}
}
