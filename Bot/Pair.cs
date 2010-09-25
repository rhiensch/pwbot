namespace Bot
{
	public class Pair<T1, T2>
	{
		public Pair()
		{
		}

		public Pair(T1 first, T2 second)
		{
			First = first;
			Second = second;
		}

		private T1 first;
		private T2 second;
		public T1 First { get { return first; } set { first = value; } }
		public T2 Second { get { return second; } set { second = value; } }
	};
}
