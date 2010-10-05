using System;
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
				"P 2 2 1 10 5#1\n" +
				"P 3 3 0 30 3#2\n" +
				"P 4 4 0 50 5#3\n" +
				"P 5 5 0 40 4#4\n" +
				"P 6 6 2 10 4#5\n";

			PlanetWars pw = new PlanetWars(message);
			Planets targetPlanets = FirstMoveAdviser.GetTargetPlanets(pw.NeutralPlanets(), 100);

			Assert.AreEqual(2, targetPlanets.Count);
			Assert.AreEqual(3+4, targetPlanets[0].PlanetID() + targetPlanets[1].PlanetID());
		}
	}
}
