using System;

namespace Bot
{
	public static class Config
	{
		static Config()
		{
			doInvadeKoef = 1.2;
			invadeSendMoreThanEnemyCanDefend = false;
		}

		public static int MinShipsOnPlanetsAfterDefend { get { return 0; } }
		public static int MinShipsOnPlanetsAfterInvade { get { return 1; } }
		public static int MinShipsOnPlanetsAfterAttack { get { return 1; } }
		public static int StartDefendDistance { get { return 15; } }
		public static int ExtraTurns { get { return 3; } }
		public static int GrowsRateKoef { get { return 4; } }
		public static int DistanceKoef { get { return 1200; } }
		public static int NumShipsKoef { get { return 60; } }
		public static int ScoreKoef { get { return 1000; } }
		public static int ScoreTurns { get { return 30; } }
		public static int CriticalTimeInMilliseconds { get { return 500; } }
		private static double doInvadeKoef;
		private static bool invadeSendMoreThanEnemyCanDefend;

		public static double DoInvadeKoef
		{
			get { return doInvadeKoef; }
			set { doInvadeKoef = value; }
		}

		public static bool InvadeSendMoreThanEnemyCanDefend
		{
			get 
			{
				return invadeSendMoreThanEnemyCanDefend;
			}
			set 
			{
				invadeSendMoreThanEnemyCanDefend = value;
			}
		}
	}
}
