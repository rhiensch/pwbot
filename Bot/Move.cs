namespace Bot
{
	public class Move
	{
		private int sourceID;
		private int destinationID;
		private int numShips;
		public int SourceID { get { return sourceID; } private set { sourceID = value; } }
		public int DestinationID { get { return destinationID; } private set { destinationID = value; } }
		public int NumShips { get { return numShips; } private set { numShips = value; } }

		//Use this only for internal calculations
		private int turnsBefore;
		public int TurnsBefore { get { return turnsBefore; } set { turnsBefore = value; } }

		public Move(int sourceID, int destID, int numShips)
		{
			SourceID = sourceID;
			DestinationID = destID;
			NumShips = numShips;
			TurnsBefore = 0;
		}

		public Move(Planet source, Planet dest, int numShips)
			: this(source.PlanetID(), dest.PlanetID(), numShips)
		{
		}

		public override string ToString()
		{
			return
				"from " + SourceID + 
				" to " + DestinationID +
				" num " + NumShips +
				(TurnsBefore > 0 ? "(after " + TurnsBefore + ")" : "");
		}
	}
}
