namespace Bot
{
	public class Fleet
	{
		// Initializes a fleet.
		public Fleet(int owner,
		             int numShips,
		             int sourcePlanet,
		             int destinationPlanet,
		             int totalTripLength,
		             int turnsRemaining)
		{
			this.owner = owner;
			this.numShips = numShips;
			this.sourcePlanet = sourcePlanet;
			this.destinationPlanet = destinationPlanet;
			this.totalTripLength = totalTripLength;
			this.turnsRemaining = turnsRemaining;
		}

		public Fleet(
				int owner,
				int numShips,
				Planet source,
				Planet dest,
				int totalTripLength,
				int turnsRemaining)
			: this(
				owner,
				numShips,
				source.PlanetID(),
				dest.PlanetID(),
				totalTripLength,
				turnsRemaining)
		{
		}

		// Initializes a fleet.
		public Fleet(int owner,
		             int numShips)
		{
			this.owner = owner;
			this.numShips = numShips;
			sourcePlanet = -1;
			destinationPlanet = -1;
			totalTripLength = -1;
			turnsRemaining = -1;
		}

		// Accessors and simple modification functions. These should be mostly
		// self-explanatory.
		public int Owner()
		{
			return owner;
		}

		public int NumShips()
		{
			return numShips;
		}

		public int SourcePlanet()
		{
			return sourcePlanet;
		}

		public int DestinationPlanet()
		{
			return destinationPlanet;
		}

		public int TotalTripLength()
		{
			return totalTripLength;
		}

		public int TurnsRemaining()
		{
			return turnsRemaining;
		}

		public void RemoveShips(int amount)
		{
			numShips -= amount;
		}

		// Subtracts one turn remaining. Call this function to make the fleet get
		// one turn closer to its destination.
		public void TimeStep()
		{
			if (turnsRemaining > 0)
			{
				--turnsRemaining;
			}
			else
			{
				turnsRemaining = 0;
			}
		}

		private readonly int owner;
		private int numShips;
		private readonly int sourcePlanet;
		private readonly int destinationPlanet;
		private readonly int totalTripLength;
		private int turnsRemaining;

		public Fleet(Fleet f)
		{
			owner = f.owner;
			numShips = f.numShips;
			sourcePlanet = f.sourcePlanet;
			destinationPlanet = f.destinationPlanet;
			totalTripLength = f.totalTripLength;
			turnsRemaining = f.turnsRemaining;
		}
	}
}
