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
					}
					if (moves.Count > 0) doBreak = false;

					moves = invadeAdviser.Run();
					if (!CheckTime(stopwatch)) return;
					foreach (Move move in moves)
					{
						Context.IssueOrder(move);
						if (!CheckTime(stopwatch)) return;
					}
					if (moves.Count > 0) doBreak = false;

					moves = attackAdviser.Run();
					if (!CheckTime(stopwatch)) return;
					foreach (Move move in moves)
					{
						Context.IssueOrder(move);
						if (!CheckTime(stopwatch)) return;
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

			/*
				// (1) If we currently have a fleet in flight, just do nothing.
				if (pw.MyFleets().Count >= 1) {
					return;
				}
				// (2) Find my strongest planet.
				Planet source = null;
				double sourceScore = Double.MinValue;
				foreach (Planet p in pw.MyPlanets()) {
					double score = (double)p.NumShips();
					if (score > sourceScore) {
						sourceScore = score;
						source = p;
					}
				}
				// (3) Find the weakest enemy or neutral planet.
				Planet dest = null;
				double destScore = Double.MinValue;
				foreach (Planet p in pw.NotMyPlanets()) {
					double score = 1.0 / (1 + p.NumShips());
					if (score > destScore) {
						destScore = score;
						dest = p;
					}
				}
				// (4) Send half the ships from my strongest planet to the weakest
				// planet that I do not own.
				if (source != null && dest != null) {
					int numShips = source.NumShips() / 2;
					pw.IssueOrder(source, dest, numShips);
				}*/
		}

		/*public static void InitLog()
		{
			byte[] byteData = Encoding.ASCII.GetBytes("Start\n");
			FileStream fs = new FileStream("mylog.txt", FileMode.Create, FileAccess.Write);
			fs.Write(byteData, 0, byteData.Length);
			fs.Close();
		}

		public static void Log(string text)
		{
			byte[] byteData = Encoding.ASCII.GetBytes(text);
			FileStream fs = new FileStream("mylog.txt", FileMode.Append);
			fs.Write(byteData, 0, byteData.Length);
			fs.Close();
			//fs.Write(text.ToCharArray(0, text.Length), 0, text.Length);
		}*/

		public static void Main()
		{
			//InitLog();
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
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
							//Log(line + '\n');
							line = line.Trim();
							if (line.Equals("go"))
							{
								//Log(message);
								//Console.WriteLine("Start");
								MyBot bot = new MyBot(new PlanetWars(message));
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

