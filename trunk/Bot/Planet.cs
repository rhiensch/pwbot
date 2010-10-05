namespace Bot
{
	public class Planet
	{
		// Initializes a planet.
		public Planet(int planetID,
		              int owner,
		              int numShips,
		              int growthRate,
		              double x,
		              double y)
		{
			this.planetID = planetID;
			this.owner = owner;
			this.numShips = numShips;
			this.growthRate = growthRate;
			this.x = x;
			this.y = y;
		}

		// Accessors and simple modification functions. These should be mostly
		// self-explanatory.
		public int PlanetID()
		{
			return planetID;
		}

		public int Owner()
		{
			return owner;
		}

		public int NumShips()
		{
			return numShips;
		}

		public int GrowthRate()
		{
			return growthRate;
		}

		public double X()
		{
			return x;
		}

		public double Y()
		{
			return y;
		}

		public void Owner(int newOwner)
		{
			owner = newOwner;
		}

		public void NumShips(int newNumShips)
		{
			numShips = newNumShips;
		}

		public void AddShips(int amount)
		{
			numShips += amount;
		}

		public void RemoveShips(int amount)
		{
			numShips -= amount;
		}

		private readonly int planetID;
		private int owner;
		private int numShips;
		private readonly int growthRate;
		private readonly double x;
		private readonly double y;

		public Planet(Planet p)
		{
			planetID = p.planetID;
			owner = p.owner;
			numShips = p.numShips;
			growthRate = p.growthRate;
			x = p.x;
			y = p.y;
		}

		public override string ToString()
		{
			return planetID.ToString();
		}
	}
}
