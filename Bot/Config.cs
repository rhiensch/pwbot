namespace Bot
{
	public static class Config
	{
		static Config()
		{
			_invokeDistanceForDefend = 15;
			_invokeDistanceForInvade = 10;
			_invokeDistanceForAttack = 15;
		}

		private static int _invokeDistanceForDefend;
		private static int _invokeDistanceForInvade;
		private static int _invokeDistanceForAttack;
		public static int InvokeDistanceForDefend { get { return _invokeDistanceForDefend; } set { _invokeDistanceForDefend = value; } }
		public static int InvokeDistanceForInvade { get { return _invokeDistanceForInvade + _invokeDistanceForInvadeModifier; } set { _invokeDistanceForInvade = value; } }
		public static int InvokeDistanceForAttack { get { return _invokeDistanceForAttack; } set { _invokeDistanceForAttack = value; } }

		private static int _invokeDistanceForInvadeModifier;
		public static void IncInvadeDistance()
		{
			_invokeDistanceForInvadeModifier++;
		}

		public static void ResetInvadeDistance()
		{
			_invokeDistanceForInvadeModifier = 0;
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
		public static double DoInvadeKoef { get { return 1.5; } }
	}
}
