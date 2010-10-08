namespace Bot
{
	public class Step
	{
		public Step(int fromTurn, int toTurn, int numShips)
		{
			this.fromTurn = fromTurn;
			this.toTurn = toTurn;
			this.numShips = numShips;
		}

		private int fromTurn;
		public int FromTurn
		{
			get { return fromTurn; }
			set { fromTurn = value; }
		}

		private int toTurn;
		public int ToTurn
		{
			get { return toTurn; }
			set { toTurn = value; }
		}

		private int numShips;
		public int NumShips
		{
			get { return numShips; }
			set { numShips = value; }
		}

		public override string ToString()
		{
			return "fromTurn: " + FromTurn + " toTurn: " + ToTurn + " numShips:" + NumShips;
		}
	}
}
