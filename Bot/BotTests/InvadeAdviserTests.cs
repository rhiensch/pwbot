using System;
using System.Diagnostics;
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
	/// Summary description for InvadeAdviserTests
	/// </summary>
	[TestClass]
	public class InvadeAdviserTests
	{
		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
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
			Moves moves = adviser.Run();

			Assert.AreEqual(0, moves.Count);
		}

		[TestMethod]
		public void TestInvadeNeutralPlanet()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.6135908004 11.6587374197 0 119 0#0\n" +
				"P 1.2902863101 9.04078582767 1 40 5#1\n" +
				"P 21.9368952907 14.2766890117 2 100 5#2\n" +
				"P 2.64835767563 10.2659924733 1 21 4#3\n" +
				"P 11.5788239251 10.05148236609 0 21 0#4\n" +
				"go\n");
			
			IAdviser adviser = new InvadeAdviser(planetWars);
			Moves moves = adviser.Run();

			Assert.AreEqual(2, moves.Count);
			Assert.AreEqual(4, moves[0].DestinationID);
			Assert.AreEqual(4, moves[1].DestinationID);
			Assert.AreEqual(31, moves[0].NumSheeps + moves[1].NumSheeps);
		}

		[TestMethod]
		public void TestInvadeNeutralPlanetOnRealMap()
		{
			PlanetWars planetWars = new PlanetWars(
				"P 11.8039955755 11.2157212798 0 37 3\n" +
				"P 9.31956732508 21.8088737532 1 100 5\n" +
				"P 14.2884238258 0.622568806433 2 100 5\n" +
				"P 11.8654926942 5.2737846552 0 81 3\n" +
				"P 11.7424984567 17.1576579045 0 81 3\n" +
				"P 4.25409258443 0.0 0 22 1\n" +
				"P 19.3538985665 22.4314425597 0 22 1\n" +
				"P 14.7436138612 22.3240014889 0 83 3\n" +
				"P 8.86437728973 0.107441070771 0 83 3\n" +
				"P 19.8543468498 0.711933891201 0 84 1\n" +
				"P 3.75364430115 21.7195086685 0 84 1\n" +
				"P 8.86481414847 9.73662367883 0 12 5\n" +
				"P 14.7431770024 12.6948188808 0 12 5\n" +
				"P 0.0 10.8098889721 0 59 2\n" +
				"P 23.6079911509 11.6215535875 0 59 2\n" +
				"P 20.3967683707 15.5228613809 0 59 3\n" +
				"P 3.21122278016 6.90858117873 0 59 3\n" +
				"P 17.0287479269 6.65976901033 0 4 2\n" +
				"P 6.57924322402 15.7716735493 0 4 2\n" +
				"P 0.782927536597 19.6075053882 0 55 1\n" +
				"P 22.8250636143 2.82393717142 0 55 1\n" +
				"P 2.60103334076 13.172383428 0 58 3\n" +
				"P 21.0069578101 9.25905913169 0 58 3\n" +
				"go\n");

			IAdviser adviser = new InvadeAdviser(planetWars);

			Stopwatch stopwatch = new Stopwatch();

			stopwatch.Start();
			Moves moves = adviser.Run();
			stopwatch.Stop();

			Assert.IsTrue(stopwatch.Elapsed.TotalMilliseconds < 1000);
			Assert.IsTrue(moves.Count > 0);
			Assert.IsTrue(planetWars.GetPlanet(moves[0].SourceID).Owner() == 1);
			Assert.IsTrue(planetWars.GetPlanet(moves[0].DestinationID).Owner() == 0);
			Assert.IsTrue(planetWars.GetPlanet(moves[0].SourceID).NumShips() + moves[0].NumSheeps > moves[0].NumSheeps);
		}
	}
}
