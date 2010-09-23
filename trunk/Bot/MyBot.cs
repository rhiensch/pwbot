#define DEBUG

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class MyBot
	{
		public PlanetWars Context { get; private set; }
		public MyBot(PlanetWars planetWars)
		{
			Context = planetWars;
		}

		private static bool CheckTime(Stopwatch stopwatch)
		{
			if (stopwatch.ElapsedMilliseconds > 950) return false;
			return true;
		}

#if DEBUG
		private void LogMove(string prefix, Move move)
		{
			Logger.Log(
				"  " + prefix + 
				" " + move + 
				" distance " + 
				Convert.ToString(Context.Distance(move.SourceID, move.DestinationID)));
		}
#endif

		public void DoTurn()
		{
			if (_turn > 20) return;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				IAdviser defendAdviser = new DefendAdviser(Context);
				IAdviser invadeAdviser = new InvadeAdviser(Context);
				IAdviser attackAdviser = new AttackAdviser(Context);

				while (true)
				{
					if (!RunAdviser(invadeAdviser)) break;
					if (!CheckTime(stopwatch)) return;

					/*bool doBreak = !RunAdviser(defendAdviser);
					if (!CheckTime(stopwatch)) return;

					doBreak = doBreak && !RunAdviser(invadeAdviser);
					if (!CheckTime(stopwatch)) return;

					doBreak = doBreak && !RunAdviser(attackAdviser);
					if (!CheckTime(stopwatch)) return;

					if (doBreak) break;*/
				}
				stopwatch.Stop();
			}
			finally
			{
				Context.FinishTurn();
			}
		}

		private bool RunAdviser(IAdviser adviser)
		{
			Moves moves = adviser.Run();
			foreach (Move move in moves)
			{
				Context.IssueOrder(move);
				#if DEBUG
				LogMove(adviser.GetAdviserName(), move);
				#endif
			}
			return moves.Count > 0;
		}

		private static int _turn;

		public static void Main()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;

			_turn = 0;
			string line = "";
			string message = "";
			#if DEBUG
			Logger.Log("\n\n\nNew Game\n\n\n");
			#endif
			try
			{
				int c;
				while ((c = Console.Read()) >= 0)
				{
					switch (c)
					{
						case '\n':
							line = line.Trim();
							if (line.Equals("go"))
							{
								PlanetWars pw = new PlanetWars(message);
								#if DEBUG
								Logger.Log(
									"Turn " + Convert.ToString(++_turn) + 
									"(" +
									"ships " + 
									Convert.ToString(pw.MyTotalShipCount) + "/" + Convert.ToString(pw.EnemyTotalShipCount) + " " +
									"planets " +
									Convert.ToString(pw.MyPlanets().Count) + "/" + Convert.ToString(pw.EnemyPlanets().Count) + " " +
									"prod " +
									Convert.ToString(pw.MyProduction) + "/" + Convert.ToString(pw.EnemyProduction) + " " +
									")");
								#endif
								//Logger.Log(message);
								MyBot bot = new MyBot(pw);
								bot.DoTurn();
								message = "";
							}
							else
							{
								message += line + "\n";
							}
							line = "";
							break;
						default:
							line += (char) c;
							break;
					}
				}
			}
			catch (Exception)
			{
				// Owned.
			}
		}
	}
}

