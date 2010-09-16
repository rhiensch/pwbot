namespace Bot
{
	public class Move
	{
		public int SourceID { get; private set; }
		public int DestinationID { get; private set; }
		public int NumSheeps { get; private set; }

		public Move(int sourceID, int destID, int numSheeps)
		{
			SourceID = sourceID;
			DestinationID = destID;
			NumSheeps = numSheeps;
		}
	}
}
