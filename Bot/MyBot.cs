#define LOG

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Moves = System.Collections.Generic.List<Bot.Move>;
using Planets = System.Collections.Generic.List<Bot.Planet>;

namespace Bot
{
	public class MyBot
	{
		public static bool DoCheckTime { get; set; }

		public PlanetWars Context { get; private set; }

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
					FirstMoveAdviser firstMoveAdviser = new FirstMoveAdviser(Context);
					FirstMoveAdviser.CheckTime checkTime = CheckTime;
					firstMoveAdviser.CheckTimeFunc = checkTime;
					RunAdviser(firstMoveAdviser);
					return;
				}

				if (Context.MyPlanets().Count == 0) return;

				DefendAdviser defendAdviser = new DefendAdviser(Context);
				InvadeAdviser invadeAdviser = new InvadeAdviser(Context);
				AttackAdviser attackAdviser = new AttackAdviser(Context);
				StealAdviser stealAdviser = new StealAdviser(Context);
				//AntiCrisisAdviser antiCrisisAdviser = new AntiCrisisAdviser(Context);

				RunAdviser(defendAdviser);
				if (!CheckTime()) return;

				Config.AttackSendMoreThanEnemyCanDefend = true;
				Config.UseSectorsForFront = true;
				if (Context.MyFutureProduction < Context.EnemyFutureProduction ||
					((Context.MyFutureProduction == Context.EnemyFutureProduction) &&  
					 (Context.MyTotalShipCount < Context.EnemyTotalShipCount)))
				{
					if (turn - lastMove[attackAdviser.GetAdviserName()] > Config.IdleTurns &&
						turn - lastMove[invadeAdviser.GetAdviserName()] > Config.IdleTurns &&
						turn - lastMove[stealAdviser.GetAdviserName()] > Config.IdleTurns)
					{
						//Config.UseSectorsForFront = false;
						//Config.AttackSendMoreThanEnemyCanDefend = false;
#if LOG
						Logger.Log("UseSectorsForFront = false");
#endif
						//antiCrisisAdviser.Attack = Context.MyTotalShipCount < Context.EnemyTotalShipCount;
						//RunAdviser(antiCrisisAdviser);
						//if (!CheckTime()) return;
					}
				}

				RunAdviser(stealAdviser);
				if (!CheckTime()) return;

				Config.InvadeSendMoreThanEnemyCanDefend = true;//(Context.MyProduction > Context.EnemyProduction*Config.DoInvadeKoef);
				RunAdviser(invadeAdviser);
				if (!CheckTime()) return;

				RunAdviser(attackAdviser);
				if (!CheckTime()) return;
			}
			finally
			{
				try
				{
					if (turn == 1)
					{
						MakeMoves(setList);
						setList.Clear();
					}
					else
					{
						SelectAndMakeMoves();
						if (CheckTime())
						{
							//try
							{
								SupplyAdviser supplyAdviser = new SupplyAdviser(Context);
								RunAdviser(supplyAdviser);

								if (CheckTime())
								{
									MakeMoves(setList);
									setList.Clear();
								}
							}
							//catch
							{
								//Logger.Log("exception: " + e.Message);
							}
						}
					}
						
					

				}
				finally
				{
					Context.FinishTurn();
				}
			}
		}

		private List<MovesSet> setList;
		private void RunAdviser(IAdviser adviser)
		{
			if (setList == null) setList = new List<MovesSet>();

			List<MovesSet> moves = adviser.RunAll();

			setList.AddRange(moves);
		}

		private void MakeMoves(List<MovesSet> set)
		{
			if (set.Count > 1) set.Sort(new Comparer(null).CompareSetScoreGT);
			foreach (MovesSet movesSet in set)
			{
				bool isPossible = true;
				Moves moves = movesSet.GetMoves();
				foreach (Move move in moves)
				{
					isPossible = Context.IsValid(move);
					int canSend = Context.CanSend(Context.GetPlanet(move.SourceID), move.TurnsBefore);
					isPossible = isPossible && (move.NumShips <= canSend);
					if (isPossible) continue;
					break;
				}
				if (!isPossible) continue;
				if (!lastMove.ContainsKey(movesSet.AdviserName)) lastMove.Add(movesSet.AdviserName, turn);
				else lastMove[movesSet.AdviserName] = turn;
				Context.IssueOrder(movesSet);
			}
		}

		private void SelectAndMakeMoves()
		{
			if (setList == null) return;
			int n = setList.Count;

			if (n == 0) return;

			List<List<MovesSet>> sets = new List<List<MovesSet>>();

			int size = (1 << n);
			for (int i = 0; i < size; i++)
			{
				if (!CheckTime()) break;

				List<MovesSet> currentSetList = new List<MovesSet>();
				Moves totalMoves = new Moves();
				//int invadeNumber = Config.MaxInvades;

				for (int j = 0; j < n; j++)
				{
					if (!CheckTime()) break;

					if ((i & (1 << j)) <= 0) continue;

					MovesSet set = setList[j];

					//if (setList[j].AdviserName == "Invade") invadeNumber--;
					//if (invadeNumber < 0) break;

					Logger.Log("set" + set);

					currentSetList.Add(set);

					Moves moves = set.GetMoves();
					foreach (Move move in moves)
					{
						if (move.TurnsBefore > 0) continue;

						bool found = false;
						foreach (Move t in totalMoves)
						{
							if (t.SourceID != move.SourceID) continue;
							found = true;
							t.AddShips(move.NumShips);
							break;
						}
						if (!found)
						{
							totalMoves.Add(new Move(move));
						}
					}
				}
				bool isValid = totalMoves.All(totalMove => Context.IsValid(totalMove));
				if (!isValid)
					foreach (Move totalMove in totalMoves)
					{
						if (!Context.IsValid(totalMove))
						{
							Logger.Log("InValid: " + totalMove);
							break;
						}
					} 

				if (!isValid) continue;
				sets.Add(currentSetList);
			}

			setList.Clear();

			if (sets.Count > 1)
			{
				sets.Sort(new Comparer(null).CompareSetListScoreGT);
			}

			if (sets.Count > 0)
			{
				MakeMoves(sets[0]);
			}
		}

		private static int turn;
		private static MyBot bot;
		private static DateTime startTime;

		private static bool CheckTime()
		{
			if (!DoCheckTime) return true;
			bool res = (DateTime.Now - startTime).TotalMilliseconds < Config.CriticalTimeInMilliseconds;
#if LOG
			if (!res) Logger.Log("Timeout!!!");
#endif
			return res;
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
								turn++;
								Config.CurrentTurn = turn;
								PlanetWars pw = new PlanetWars(message);
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

