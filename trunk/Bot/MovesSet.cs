﻿using System;
using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public class MovesSet
	{
		private readonly PlanetWars context;

		private readonly Moves moves;
		public double Score;
		public string AdviserName;
		public double AverageDistance;
		public int MaxDistance;
		public int MinDistance;
		public int SumDistance;
		public int SummaryNumShips;

		public Moves GetMoves()
		{
			Moves result = new Moves(moves);
			return result;
		}

		public void AddMove(Move newMove)
		{
			SummaryNumShips += newMove.NumSheeps;
			int newDistance = context.Distance(newMove.SourceID, newMove.DestinationID);
			if (newDistance > MaxDistance) MaxDistance = newDistance;
			if (newDistance < MinDistance) MinDistance = newDistance;
			SumDistance += newDistance;
			
			moves.Add(newMove);

			AverageDistance = SumDistance / (double)moves.Count;
		}

		public MovesSet(Moves movesSet, double score, string adviserName, PlanetWars context)
		{
			this.context = context;
			MaxDistance = Int32.MinValue;
			MinDistance = Int32.MaxValue;
			AverageDistance = 0;
			SummaryNumShips = 0;

			moves = new Moves();
			foreach (Move move in movesSet)
			{
				AddMove(move);
			}
			
			Score = score;
			AdviserName = adviserName;
		}

		public override string ToString()
		{
			string result = 
				AdviserName + " " + " moves: " + 
				moves.Count + ", score: " + 
				string.Format("{0:F}", Score);

			for (int i = 0; i < moves.Count; i++)
			{
				result += " (" + i + ": " + moves[i] + ")";
			}
			return result;
		}

		public Move GetMove(int index)
		{
			if (index >= 0 && index < moves.Count) return moves[index];
			return null;
		}
	}
}