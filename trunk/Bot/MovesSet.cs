using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class MovesSet
	{
		public Moves Moves;
		public double Score;
		public string AdviserName;

		public MovesSet(Moves movesSet, double score, string adviserName)
		{
			Moves = movesSet;
			Score = score;
			AdviserName = adviserName;
		}

		public override string ToString()
		{
			string result = 
				AdviserName + " " + " moves: " + 
				Moves.Count + ", score: " + 
				string.Format("{0:F}", Score);

			for (int i = 0; i < Moves.Count; i++)
			{
				result += " (" + i + ": " + Moves[i] + ")";
			}
			return result;
		}
	}
}
