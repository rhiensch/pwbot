using System;

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

		public override string ToString()
		{
			return
				"from " + Convert.ToString(SourceID) + 
				" to " + Convert.ToString(DestinationID) +
				" num " + Convert.ToString(NumSheeps);
		}
	}
}
