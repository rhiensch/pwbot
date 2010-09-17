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
			Moves moves = adviser.Run();

			Assert.AreEqual(0, moves.Count);
		}
	}
}
