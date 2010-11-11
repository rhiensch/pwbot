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
	/// Summary description for MyBotTests
	/// </summary>
	[TestClass]
	public class MyBotTests
	{
		[TestInitialize]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
		}

		[TestMethod]
		public void TestCorrectWorkWhenNoPlanets()
		{
			const string message =
				"P 2 2 2 10 0#0\n" +
				"P 4 1 2 10 5#1\n" +
				"P 5 6 2 10 5#2\n" +
				"P 0 0 2 10 4#3\n" +
				"P 2 5 2 10 4#4\n" +
				"P 3 3 2 10 4#5\n" +
				"F 1 1 2 4 4 4\n";
			PlanetWars pw = new PlanetWars(message);
			MyBot bot = new MyBot(pw);

			try
			{
				bot.DoTurn();
			}
			catch
			{
				Assert.IsTrue(false);
			}
		}

		[TestMethod]
		public void TestCorrectWorkWhenNoPlanets2()
		{
			const string message =
				"P 11.0441665408 10.8983039256 2 11 1\n" +
				"P 13.467365867 10.9585014986 2 5 5\n" +
				"P 8.62096721464 10.8381063525 2 7 5\n" +
				"P 13.00123889 8.56367028965 0 72 1\n" +
				"P 9.08709419168 13.2329375615 0 72 1\n" +
				"P 20.9725608721 19.291278458 0 76 1\n" +
				"P 1.11577220953 2.50532939321 0 76 1\n" +
				"P 14.6516779553 13.588908743 2 1 1\n" +
				"P 7.43665512641 8.20769910815 2 2 1\n" +
				"P 6.65225859067 13.5603524424 2 36 5\n" +
				"P 15.436074491 8.23625540874 2 12 5\n" +
				"P 16.8971315488 9.71928017689 0 24 2\n" +
				"P 5.19120153291 12.0773276743 2 14 2\n" +
				"P 22.0883330817 2.6022889358 0 74 1\n" +
				"P 0 19.1943189154 0 74 1\n" +
				"P 16.9977626441 21.7966078512 0 3 1\n" +
				"P 5.09057043753 0 2 1 1\n" +
				"P 11.8417829975 12.6086008811 2 2 2\n" +
				"P 10.2465500842 9.18800697004 0 76 2\n" +
				"P 16.8074409487 7.20887137732 0 76 2\n" +
				"P 5.28089213293 14.5877364739 0 76 2\n" +
				"P 18.1132657811 6.81466715526 0 82 3\n" +
				"P 3.97506730062 14.9819406959 0 82 3\n" +
				"F 2 1 16 8 9 1\n" +
				"F 2 1 16 8 9 2\n" +
				"F 2 1 16 8 9 3\n" +
				"F 1 4 10 15 14 9\n" +
				"F 2 1 16 8 9 4\n" +
				"F 2 2 8 12 5 1\n" +
				"F 2 1 16 8 9 5\n" +
				"F 1 4 17 15 11 7\n" +
				"F 2 1 16 8 9 6\n" +
				"F 2 17 2 9 4 1\n" +
				"F 2 2 8 2 3 1\n" +
				"F 2 2 1 7 3 1\n" +
				"F 2 12 2 0 3 1\n" +
				"F 2 1 16 8 9 7\n" +
				"F 2 14 0 1 3 2\n" +
				"F 2 13 1 10 4 3\n" +
				"F 2 5 2 0 3 2\n" +
				"F 2 3 7 1 3 2\n" +
				"F 2 3 7 15 9 8\n" +
				"F 2 2 8 10 8 7\n" +
				"F 2 6 9 2 4 3\n" +
				"F 2 25 10 11 3 2\n" +
				"F 2 1 16 8 9 8\n" +
				"F 2 2 17 1 3 2\n";

			PlanetWars pw = new PlanetWars(message);
			MyBot bot = new MyBot(pw);

			try
			{
				Config.IdleTurns = -1;
				bot.DoTurn();
			}
			catch
			{
				Assert.IsTrue(false);
			}
		}

		[TestMethod]
		public void TestDoAttack()
		{
			PlanetWars pw = new PlanetWars(
				"P 10.7527454812 11.8062363634 1 16 5\n" +
				"P 7.71825346006 19.2282675745 1 9 5\n" +
				"P 13.7872375024 4.3842051524 1 17 5\n" +
				"P 16.5885619084 4.19029815187 1 5 3\n" +
				"P 4.91692905402 19.422174575 1 14 3\n" +
				"P 17.1016331607 8.0105449784 1 3 3\n" +
				"P 4.40385780175 15.6019277485 1 17 3\n" +
				"P 4.79266235586 21.5213595465 1 2 2\n" +
				"P 16.7128286066 2.09111318038 1 2 2\n" +
				"P 18.0296511565 14.1785383486 1 6 4\n" +
				"P 3.47583980592 9.43393437829 1 6230 4\n" +
				"P 6.5168372747 3.38746940068 1 20 2\n" +
				"P 14.9886536877 20.2250033262 1 3 2\n" +
				"P 20.6147572889 14.2829339055 1 4 4\n" +
				"P 0.890733673483 9.32953882139 1 443 4\n" +
				"P 0 0 1 1 1\n" +
				"P 21.5054909624 23.6124727269 1 1 1\n" +
				"P 5.14922220606 9.48382106252 1 22 2\n" +
				"P 16.3562687564 14.1286516644 1 15 2\n" +
				"P 8.40774862412 22.1804224379 1 4 4\n" +
				"P 13.0977423383 1.43205028894 1 4 4\n" +
				"P 19.4279066762 13.2056512821 1 2 2\n" +
				"P 2.07758428622 10.4068214448 2 13 2\n" +
				"go\n");

			MyBot bot = new MyBot(pw);
			MyBot.DoCheckTime = false;

			
			bot.DoTurn();
			Assert.IsTrue(pw.MyFleetsGoingToPlanet(pw.EnemyPlanets()[0]).Count > 0);
		}
	}
}
