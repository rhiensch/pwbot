namespace Bot
{
	public static class Config
	{
		static Config()
		{
			invokeDistanceForDefend = 15;
			invokeDistanceForInvade = 10;
			invokeDistanceForAttack = 15;
			doInvadeKoef = 1.5;
		}

		private static int invokeDistanceForDefend;
		private static int invokeDistanceForInvade;
		private static int invokeDistanceForAttack;
		public static int InvokeDistanceForDefend { get { return invokeDistanceForDefend; } set { invokeDistanceForDefend = value; } }
		public static int InvokeDistanceForInvade { get { return invokeDistanceForInvade + invokeDistanceForInvadeModifier; } set { invokeDistanceForInvade = value; } }
		public static int InvokeDistanceForAttack { get { return invokeDistanceForAttack; } set { invokeDistanceForAttack = value; } }

		private static int invokeDistanceForInvadeModifier;
		public static void IncInvadeDistance()
		{
			invokeDistanceForInvadeModifier++;
		}

		public static void ResetInvadeDistance()
		{
			invokeDistanceForInvadeModifier = 0;
		}

		public static int MinShipsOnPlanetsAfterDefend { get { return 0; } }
		public static int MinShipsOnPlanetsAfterInvade { get { return 1; } }
		public static int MinShipsOnPlanetsAfterAttack { get { return 1; } }
		public static int StartDefendDistance { get { return 15; } }
		public static int ExtraTurns { get { return 3; } }
		public static int GrowsRateKoef { get { return 4; } }
		public static int DistanceKoef { get { return -2; } }
		public static int NumShipsKoef { get { return 1; } }
		public static int CriticalTimeInMilliseconds { get { return 500; } }
		private static double doInvadeKoef;
		public static double DoInvadeKoef
		{
			get { return doInvadeKoef; }
			set { doInvadeKoef = value; }
		}
	}
}
