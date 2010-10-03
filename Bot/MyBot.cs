#define DEBUG

using System;
using System.Globalization;
using System.Threading;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class MyBot
	{
		private PlanetWars context;
		public PlanetWars Context
		{
			get { return context; }
			private set { context = value; }
		}

		public MyBot(PlanetWars planetWars)
		{
			Context = planetWars;
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
				DefendAdviser defendAdviser = new DefendAdviser(Context);
				InvadeAdviser invadeAdviser = new InvadeAdviser(Context);
				AttackAdviser attackAdviser = new AttackAdviser(Context);
				SupplyAdviser supplyAdviser = new SupplyAdviser(Context);
				StealAdviser stealAdviser = new StealAdviser(Context);

				do
				{
					if (!defendAdviser.IsWorkFinished) RunAdviser(defendAdviser);
					if (!CheckTime()) return;

					if (!stealAdviser.IsWorkFinished) RunAdviser(stealAdviser);
					if (!CheckTime()) return;

					if (!invadeAdviser.IsWorkFinished) RunAdviser(invadeAdviser);
					if (!CheckTime()) return;

					if (!attackAdviser.IsWorkFinished) RunAdviser(attackAdviser);
					if (!CheckTime()) return;

				} while (
					!defendAdviser.IsWorkFinished ||
					!invadeAdviser.IsWorkFinished ||
					!attackAdviser.IsWorkFinished ||
					!stealAdviser.IsWorkFinished);

				Planets myPlanets = Context.MyPlanets();
				foreach (Planet myPlanet in myPlanets)
				{
					supplyAdviser.SupplyPlanet = myPlanet;
					RunAdviser(supplyAdviser);
					if (!CheckTime()) return;
				}
				
			}
			finally
			{
				Context.FinishTurn();
			}
		}

		private void RunAdviser(IAdviser adviser)
		{
			Moves moves = adviser.Run();
			if (moves.Count == 0) return;
			foreach (Move move in moves)
			{
#if DEBUG
				LogMove(adviser.GetAdviserName(), move);
#endif
				Context.IssueOrder(move);
			}
		}

		private static int turn;
		private static MyBot bot;
		private static DateTime startTime;

		private static bool CheckTime()
		{
			return (DateTime.Now - startTime).TotalMilliseconds < Config.CriticalTimeInMilliseconds;
		}

		public static void Main()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;

			turn = 0;
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
									"Turn " + Convert.ToString(++turn) + 
									"(" +
									"ships " + 
									Convert.ToString(pw.MyTotalShipCount) + "/" + Convert.ToString(pw.EnemyTotalShipCount) + " " +
									"planets " +
									Convert.ToString(pw.MyPlanets().Count) + "/" + Convert.ToString(pw.EnemyPlanets().Count) + " " +
									"prod " +
									Convert.ToString(pw.MyProduction) + "/" + Convert.ToString(pw.EnemyProduction) + " " +
									")");
								if (turn == 180) Logger.Log(message);
								#endif
								
								if (bot == null)
									bot = new MyBot(pw);
								else
									bot.Context = pw;
								bot.DoTurn();
#if DEBUG
								Logger.Log("  Turn time: " + (DateTime.Now - startTime).TotalMilliseconds);
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
								startTime = DateTime.Now;
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
	}
}

