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
	public class StealAdviserTests
	{
		[TestInitialize]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestSteal()
		{
			const string message =
				"P 0 0 1 1 0#0\n" +
				"P 2 2 1 10 5#1\n" +
				"P 3 3 0 10 3#2\n" +
				"P 4 4 0 10 5#3\n" +
				"P 5 5 0 10 4#4\n" +
				"P 6 6 2 10 4#5\n" +
				"F 2 12 5 2 4 2\n" +
				"F 2 12 5 3 4 2\n" + 
				"F 2 12 5 4 4 2\n";
			PlanetWars pw = new PlanetWars(message);
			StealAdviser adviser = new StealAdviser(pw);

			List<MovesSet> moves = adviser.RunAll();

			Assert.AreEqual(2, moves.Count);
			Assert.AreEqual(1, moves[1].Moves.Count);
			Assert.AreEqual(3, moves[1].Moves[0].DestinationID);
		}
	}
}
