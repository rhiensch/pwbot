#undef DEBUG

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
		[TestInitialize()]
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
	}
}
