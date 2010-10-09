using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class MovesSet
	{
		public Moves Moves;
		public int Score;

		public MovesSet(Moves movesSet, int score)
		{
			Moves = movesSet;
			Score = score;
		}
	}
}
