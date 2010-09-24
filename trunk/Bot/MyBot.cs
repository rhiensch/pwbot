#define DEBUG

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Timers;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;
using Timer = System.Timers.Timer;

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
			if (stopwatch.ElapsedMilliseconds > Config.CriticalTimeInMilliseconds)
			{
#if DEBUG
				Logger.Log("Attention! Timeout is close! (" + stopwatch.ElapsedMilliseconds + ")");
#endif
				return false;
			}
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
			//if (_turn > 20) return;
			try
			{
				IAdviser defendAdviser = new DefendAdviser(Context);
				IAdviser invadeAdviser = new InvadeAdviser(Context);
				IAdviser attackAdviser = new AttackAdviser(Context);
				SupplyAdviser supplyAdviser = new SupplyAdviser(Context);

				while (true)
				{
					bool doBreak = !RunAdviser(defendAdviser);

					doBreak = doBreak && !RunAdviser(invadeAdviser);

					doBreak = doBreak && !RunAdviser(attackAdviser);

					//TODO Bug is here! Do Logging
					Planets myPlanets = Context.MyPlanets();
					foreach (Planet planet in myPlanets)
					{
						supplyAdviser.SupplyPlanet = planet;
						RunAdviser(supplyAdviser);
					}
					
					if (doBreak) break;
				}
				
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
		private static MyBot bot;
		private static readonly Timer timer = new Timer(Config.CriticalTimeInMilliseconds);
		private static readonly Stopwatch stopWatch = new Stopwatch();

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
			timer.Elapsed += OnTimedEvent;
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
								if (bot == null)
									bot = new MyBot(pw);
								else
									bot.Context = pw;
								try
								{
									bot.DoTurn();
								}
								catch(TimeoutException e)
								{
#if DEBUG
									Logger.Log(e.Message);
#endif
								}
#if DEBUG
								Logger.Log("  Turn time: " + stopWatch.ElapsedMilliseconds);
								stopWatch.Stop();
#endif
								message = "";
							}
							else
							{
								message += line + "\n";
							}
							line = "";
							break;
						default:
							if (line == "")
							{
								//start reading data
								timer.Enabled = true;
#if DEBUG
								stopWatch.Reset();
								stopWatch.Start();
#endif
							}
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

		private static void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			bot.Context.FinishTurn();
			//turnInterrupted = true;
			throw new TimeoutException("  !Turn finished by timer!");
		}

	}
}

