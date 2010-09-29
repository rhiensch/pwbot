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
				IAdviser defendAdviser = new DefendAdviser(Context);
				IAdviser invadeAdviser = new InvadeAdviser(Context);
				IAdviser attackAdviser = new AttackAdviser(Context);
				SupplyAdviser supplyAdviser = new SupplyAdviser(Context);

				bool doDefend;
				bool doAttack;
				bool doInvade;

				do
				{
					doDefend = RunAdviser(defendAdviser);
					if (!CheckTime()) return;

					doInvade = RunAdviser(invadeAdviser);
					if (!CheckTime()) return;

					doAttack = RunAdviser(attackAdviser);
					if (!CheckTime()) return;

				} while (doDefend || doAttack || doInvade);

				Planets myPlanets = Context.MyPlanets();
				foreach (Planet planet in myPlanets)
				{
					supplyAdviser.SupplyPlanet = planet;
					RunAdviser(supplyAdviser);
					if (!CheckTime()) return;
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
			if (moves.Count == 0) return false;
#if DEBUG
			//Logger.Log("  " + adviser.GetAdviserName() + ": " + moves.Count + " moves");
#endif
			foreach (Move move in moves)
			{
#if DEBUG
				LogMove(adviser.GetAdviserName(), move);
#endif
				Context.IssueOrder(move);
			}
			return true;
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
								if (turn == 12) Logger.Log(message);
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

