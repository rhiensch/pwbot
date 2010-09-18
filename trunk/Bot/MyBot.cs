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

		private bool CheckTime(Stopwatch stopwatch)
		{
			if (stopwatch.ElapsedMilliseconds > 950) return false;
			return true;
		}

		private void LogMove(string prefix, Move move)
		{
			Logger.Log("  " + prefix + " " + move + " distance " + Convert.ToString(Context.Distance(move.SourceID, move.DestinationID)));
		}

		public void DoTurn()
		{
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				IAdviser defendAdviser = new DefendAdviser(Context);
				IAdviser invadeAdviser = new InvadeAdviser(Context);
				IAdviser attackAdviser = new AttackAdviser(Context);

				while (true)
				{
					bool doBreak = true;

					Moves moves = defendAdviser.Run();
					if (!CheckTime(stopwatch)) return;
					foreach (Move move in moves)
					{
						Context.IssueOrder(move);
						if (!CheckTime(stopwatch)) return;
						#if DEBUG
						LogMove("Defend", move);
						#endif
					}
					if (moves.Count > 0) doBreak = false;

					moves = invadeAdviser.Run();
					if (!CheckTime(stopwatch)) return;
					foreach (Move move in moves)
					{
						Context.IssueOrder(move);
						if (!CheckTime(stopwatch)) return;
						#if DEBUG
						LogMove("Invade", move);
						#endif
					}
					if (moves.Count > 0) doBreak = false;

					moves = attackAdviser.Run();
					if (!CheckTime(stopwatch)) return;
					foreach (Move move in moves)
					{
						Context.IssueOrder(move);
						if (!CheckTime(stopwatch)) return;
						#if DEBUG
						LogMove("Attack", move);
						#endif
					}
					if (moves.Count > 0) doBreak = false;

					if (doBreak) break;
				}
				stopwatch.Stop();
			}
			finally
			{
				Context.FinishTurn();
			}
		}

		public static void Main()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
			string line = "";
			string message = "";
			int turnNumber = 0;
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
									"Turn " + Convert.ToString(++turnNumber) + 
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

