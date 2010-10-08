namespace Bot
{
	public class PlanetOwnerSwitch
	{
		public PlanetOwnerSwitch(int oldPlanetOwner, int newPlanetOwner, int turns)
		{
			oldOwner = oldPlanetOwner;
			newOwner = newPlanetOwner;
			turnsBefore = turns;
		}

		private int oldOwner;
		public int OldOwner
		{
			get { return oldOwner; }
			set { oldOwner = value; }
		}

		private int newOwner;
		public int NewOwner
		{
			get { return newOwner; }
			set { newOwner = value; }
		}

		private int turnsBefore;
		public int TurnsBefore
		{
			get { return turnsBefore; }
			set { turnsBefore = value; }
		}

		public override string ToString()
		{
			return 
				"old: " + oldOwner +
				" new: " + newOwner +
				" turns: " + turnsBefore;
		}
	}
}
