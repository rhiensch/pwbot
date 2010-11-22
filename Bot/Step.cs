namespace Bot
{
	public class Step
	{
		public Step(int fromTurn, int toTurn, int numShips)
		{
			FromTurn = fromTurn;
			ToTurn = toTurn;
			NumShips = numShips;
		}

		public int FromTurn { get; set; }

		public int ToTurn { get; set; }

		public int NumShips { get; set; }

		public override string ToString()
		{
			return "fromTurn: " + FromTurn + " toTurn: " + ToTurn + " numShips:" + NumShips;
		}
	}
}
