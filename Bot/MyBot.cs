#define LOG

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

		private Dictionary<string, int> lastMove;

		public MyBot(PlanetWars planetWars)
		{
			Context = planetWars;
			DoCheckTime = true;
			InitLastMove();
		}

		private void InitLastMove()
		{
			lastMove = new Dictionary<string, int>(7);

			FirstMoveAdviser firstMoveAdviser = new FirstMoveAdviser(Context);
			DefendAdviser defendAdviser = new DefendAdviser(Context);
			InvadeAdviser invadeAdviser = new InvadeAdviser(Context);
			AttackAdviser attackAdviser = new AttackAdviser(Context);
			SupplyAdviser supplyAdviser = new SupplyAdviser(Context);
			StealAdviser stealAdviser = new StealAdviser(Context);
			AntiCrisisAdviser antiCrisiAdviser = new AntiCrisisAdviser(Context);

			lastMove.Add(firstMoveAdviser.GetAdviserName(), 0);
			lastMove.Add(defendAdviser.GetAdviserName(), 0);
			lastMove.Add(invadeAdviser.GetAdviserName(), 0);
			lastMove.Add(attackAdviser.GetAdviserName(), 0);
			lastMove.Add(supplyAdviser.GetAdviserName(), 0);
			lastMove.Add(stealAdviser.GetAdviserName(), 0);
			lastMove.Add(antiCrisiAdviser.GetAdviserName(), 0);
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
				AntiCrisisAdviser antiCrisiAdviser = new AntiCrisisAdviser(Context);

				RunAdviser(defendAdviser);
				if (!CheckTime()) return;

				if (Context.MyProduction < Context.EnemyProduction)
				{
					if (turn - lastMove[attackAdviser.GetAdviserName()] > Config.IdleTurns &&
						turn - lastMove[invadeAdviser.GetAdviserName()] > Config.IdleTurns &&
						turn - lastMove[stealAdviser.GetAdviserName()] > Config.IdleTurns)
					{
						RunAdviser(antiCrisiAdviser);
						if (!CheckTime()) return;
						//Console.WriteLine("stop!");
					}
				}

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
				bool isPossible = false;
				Moves moves = movesSet.GetMoves();
				foreach (Move move in moves)
				{
					isPossible = Context.IsValid(move);
					int canSend = Context.CanSend(Context.GetPlanet(move.SourceID), move.TurnsBefore);
					isPossible = isPossible && (move.NumShips <= canSend);
					if (!isPossible) break;
				}
				if (isPossible)
				{
					if (!lastMove.ContainsKey(movesSet.AdviserName)) lastMove.Add(movesSet.AdviserName, turn);
					else lastMove[movesSet.AdviserName] = turn;
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
								turn++;
#if LOG
								Logger.Log("");
								Logger.Log(
									"Turn " + Convert.ToString(turn) +
									" (" +
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
#if LOG
								//Logger.Log("  Turn time: " + (DateTime.Now - startTime).TotalMilliseconds);
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
	}
}

