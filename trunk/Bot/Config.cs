namespace Bot
{
	public static class Config
	{
		static Config()
		{
			DoInvadeKoef = 1.2;
			IdleTurns = 15;
			ScoreTurns = 30;
			MaxInvades = 3;
			InvadeSendMoreThanEnemyCanDefend = false;
			AttackSendMoreThanEnemyCanDefend = true;
			MaxTurns = 200;
		}

		public static int MinShipsOnPlanetsAfterDefend { get { return 0; } }
		public static int MinShipsOnPlanetsAfterInvade { get { return 1; } }
		public static int MinShipsOnPlanetsAfterAttack { get { return 1; } }
		public static int StartDefendDistance { get { return 15; } }
		public static int ExtraTurns { get { return 3; } }
		public static int MaxPlanets { get { return 30; } }
		public static int GrowsRateKoef { get { return 4; } }
		public static int DistanceKoef { get { return 1200; } }
		public static int NumShipsKoef { get { return 60; } }
		public static int ScoreKoef { get { return 1000; } }
		public static int ScoreTurns { get; set; }
		public static int IdleTurns { get; set; }
		public static int MaxInvades { get; set; }
		public static int CriticalTimeInMilliseconds { get { return 800; } }

		public static double DoInvadeKoef { get; set; }

		public static bool InvadeSendMoreThanEnemyCanDefend { get; set; }
		public static bool AttackSendMoreThanEnemyCanDefend { get; set; }

		public static int CurrentTurn { get; set; }

		public static int MaxTurns { get; set; }
	}
}
