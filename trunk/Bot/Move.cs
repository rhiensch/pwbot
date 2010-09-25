namespace Bot
{
	public class Move
	{
		private int sourceID;
		private int destinationID;
		private int numSheeps;
		public int SourceID { get { return sourceID; } private set { sourceID = value; } }
		public int DestinationID { get { return destinationID; } private set { destinationID = value; } }
		public int NumSheeps { get { return numSheeps; } private set { numSheeps = value; } }

		public Move(int sourceID, int destID, int numSheeps)
		{
			SourceID = sourceID;
			DestinationID = destID;
			NumSheeps = numSheeps;
		}

		public override string ToString()
		{
			return
				"from " + SourceID + 
				" to " + DestinationID +
				" num " + NumSheeps;
		}
	}
}
