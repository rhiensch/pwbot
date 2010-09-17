namespace Bot
{
	public static class Config
	{
		public static int MinShipsOnMyPlanetsAfterDefend { get { return 5; } }
		public static int MinShipsOnMyPlanetsAfterInvade { get { return 1; } }
		public static int MinShipsOnMyPlanetsAfterAttack { get { return 10; } }
		public static int InvokeDistanceForDefend { get { return 15; } }
		public static int InvokeDistanceForInvade { get { return 10; } }
		public static int InvokeDistanceForAttack { get { return 15; } }
		public static int StartDefendDistance { get { return 15; } }
		public static int ExtraTurns { get { return 3; } }
		public static int GrowsRateKoef { get { return 4; } }
		public static int DistanceFoef { get { return -1; } }
		public static int NumShipsKoef { get { return 1; } }
	}
}
