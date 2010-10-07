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
			PlanetWars pw = new PlanetWars(message);

			IAdviser adviser = new SupplyAdviser(pw, pw.GetPlanet(0));
			Moves moves = adviser.Run();

			Assert.AreEqual(1, moves.Count);
			/*Assert.AreEqual(3, moves[0].SourceID);
			Assert.AreEqual(0, moves[0].DestinationID);
			Assert.AreEqual(10, moves[0].NumSheeps);*/
		}

		[TestMethod]
		public void TestDontSupplyFromEndangeredPlanets()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 10.9462142783 11.7571388049 1 24 4\n"+
				"P 18.033071482 20.1129893546 1 5 5\n"+
				"P 3.85935707472 3.40128825511 2 5 5\n"+
				"P 19.2826666634 9.50695040005 0 61 2\n"+
				"P 2.6097618933 14.0073272097 0 61 2\n"+
				"P 4.98803321494 6.55584398927 2 7 2\n"+
				"P 16.9043953417 16.9584336205 1 9 2\n"+
				"P 17.8198951395 5.6245678102 0 79 3\n"+
				"P 4.07253341715 17.8897097996 0 79 3\n"+
				"P 12.9362327012 0.298719287095 0 36 1\n"+
				"P 8.95619585546 23.2155583227 1 1 1\n"+
				"P 0.0 23.5142776098 0 43 1\n"+
				"P 21.8924285567 0.0 0 43 1\n"+
				"P 13.7299496073 13.8857860803 0 59 2\n"+
				"P 8.16247894941 9.62849152945 2 14 2\n"+
				"P 7.9460755228 4.99828064251 0 87 2\n"+
				"P 13.9463530339 18.5159969673 0 87 2\n"+
				"P 5.11878762985 4.42057885479 2 2 1\n"+
				"P 16.7736409268 19.093698755 1 7 1\n"+
				"P 17.7782398876 11.674772355 0 62 1\n"+
				"P 4.11418866912 11.8395052548 0 62 1\n"+
				"P 20.8113361481 21.049950329 0 19 5\n"+
				"P 1.08109240856 2.46432728074 2 5 5\n"+
				"F 2 16 14 10 14 13\n"+
				"F 1 2 0 10 12 12\n" +
				"go\n");

			/*Planets planets = planetWars.MyEndangeredPlanets(14, 0);
			Assert.IsTrue(planets.IndexOf(planetWars.GetPlanet(10)) >= 0);*/

			SupplyAdviser adviser = new SupplyAdviser(planetWars) {SupplyPlanet = planetWars.GetPlanet(10)};

			Moves moves = adviser.Run();
			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestSupplyMoreThenOnce()
		{
			const string message =
				"P 1 1 1 10 5\n" +
				"P 2 2 1 10 5\n" +
				"P 3 3 1 10 5\n" +
				"P 4 4 2 10 5\n";
			PlanetWars pw = new PlanetWars(message);

			SupplyAdviser adviser = new SupplyAdviser(pw);

			adviser.SupplyPlanet = pw.GetPlanet(0);
			Moves moves = adviser.Run();
			Assert.IsTrue(moves.Count > 0);
			Assert.AreEqual(1, moves[0].DestinationID);

			adviser.SupplyPlanet = pw.GetPlanet(1);
			moves = adviser.Run();
			Assert.IsTrue(moves.Count > 0);
			Assert.AreEqual(2, moves[0].DestinationID);
		}
	}
}
