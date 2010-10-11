using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class MovesSet
	{
		public Moves Moves;
		public double Score;

		public MovesSet(Moves movesSet, double score)
		{
			Moves = movesSet;
			Score = score;
		}
	}
}
