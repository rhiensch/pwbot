#define DEBUG

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class MyBot
	{
		private static bool doCheckTime;
		public static bool DoCheckTime
		{
			get { return doCheckTime; }
			set { doCheckTime = value; }
		}

		private PlanetWars context;
		public PlanetWars Context
		{
			get { return context; }
			private set { context = value; }
		}

		public MyBot(PlanetWars planetWars)
		{
			Context = planetWars;
			DoCheckTime = true;
		}

		public void DoTurn()
		{
			try
			{
				if (turn == 1)
				{
					RunAdviser(new FirstMoveAdviser(Context));
					return;
				}

				DefendAdviser defendAdviser = new DefendAdviser(Context);
				InvadeAdviser invadeAdviser = new InvadeAdviser(Context);
				AttackAdviser attackAdviser = new AttackAdviser(Context);
				SupplyAdviser supplyAdviser = new SupplyAdviser(Context);
				StealAdviser stealAdviser = new StealAdviser(Context);

				
				RunAdviser(defendAdviser);
				if (!CheckTime()) return;

				RunAdviser(stealAdviser);
				if (!CheckTime()) return;

				Config.InvadeSendMoreThanEnemyCanDefend = true;//(Context.MyProduction > Context.EnemyProduction*Config.DoInvadeKoef);
				RunAdviser(invadeAdviser);
				if (!CheckTime()) return;

				RunAdviser(attackAdviser);
				if (!CheckTime()) return;

				RunAdviser(supplyAdviser);
				if (!CheckTime()) return;
			}
			finally
			{
				try
				{
					SelectAndMakeMoves();
				}
				finally
				{
					Context.FinishTurn();
				}
			}
		}

		private List<MovesSet> setList;
		private void RunAdviser(BaseAdviser adviser)
		{
			if (setList == null) setList = new List<MovesSet>();

			List<MovesSet> moves = adviser.RunAll();
			setList.AddRange(moves);
		}

		private void SelectAndMakeMoves()
		{
			if (setList.Count == 0) return;

			if (setList.Count > 1) setList.Sort(new Comparer(null).CompareSetScoreGT);

			foreach (MovesSet movesSet in setList)
			{
				bool isPossible = true;
				foreach (Move move in movesSet.Moves)
				{
					isPossible = Context.IsValid(move);
					if (!isPossible) break;
				}
				if (isPossible)
				{
					Context.IssueOrder(movesSet);
				}
			}
			setList.Clear();
		}

		private static int turn;
		private static MyBot bot;
		private static DateTime startTime;

		private static bool CheckTime()
		{
			if (!DoCheckTime) return true;
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
// ReSharper disable EmptyGeneralCatchClause
			catch
// ReSharper restore EmptyGeneralCatchClause
			{
				// Owned.
			}
		}

#if DEBUG
		~MyBot()
		{
			Logger.Close();
		}
#endif
	}
}

