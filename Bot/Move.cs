namespace Bot
{
	public class Move
	{
		public int SourceID { get; private set; }
		public int DestinationID { get; private set; }
		public int NumShips { get; private set; }

		//Use this only for internal calculations
		public int TurnsBefore { get; set; }

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

		public Move(Move move)
			: this(move.SourceID, move.DestinationID, move.NumShips)
		{
		}

		public void AddShips(int addShips)
		{
			NumShips += addShips;
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
