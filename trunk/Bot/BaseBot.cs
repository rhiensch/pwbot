using System.Collections.Generic;

namespace Bot
{
	public class BaseBot
	{
		public PlanetWars Context { get; private set; }
		public BaseBot(PlanetWars planetWars)
		{
			Context = planetWars;
		}

		public int CompareNumberOfShipsLt(Planet planet1, Planet planet2)
		{
			return (planet1.NumShips() - planet2.NumShips());
		}

		public List<Planet> WeakestPlanets(List<Planet> planets, int number)
		{
			List<Planet> weakestPlanets = new List<Planet>(number);
			if (number == 1)
			{
				Planet weakestPlanet = planets[0];

				foreach (Planet planet in planets)
				{
					if (planet.NumShips() < weakestPlanet.NumShips())
					{
						weakestPlanet = planet;
					}
				}
				weakestPlanets.Add(weakestPlanet);
			}
			else if (number != 0)
			{
				List<Planet> sortedPlanets = planets;
				sortedPlanets.Sort(CompareNumberOfShipsLt);

				if (number > sortedPlanets.Count) 
				{
					number = planets.Count;
				}

				weakestPlanets = sortedPlanets.GetRange(0, number);
			}
			return weakestPlanets;
		}

		public List<Planet> StrongestPlanets(List<Planet> planets, int number)
		{
			List<Planet> weakestPlanets = new List<Planet>(number);
			if (number == 1)
			{
				Planet weakestPlanet = planets[0];

				foreach (Planet planet in planets)
				{
					if (planet.NumShips() > weakestPlanet.NumShips())
					{
						weakestPlanet = planet;
					}
				}
				weakestPlanets.Add(weakestPlanet);
			}
			else if (number != 0)
			{
				List<Planet> sortedPlanets = planets;
				sortedPlanets.Sort(CompareNumberOfShipsLt);
				sortedPlanets.Reverse();

				if (number > sortedPlanets.Count)
				{
					number = planets.Count;
				}

				weakestPlanets = sortedPlanets.GetRange(0, number);
			}

			return weakestPlanets;
		}
	}
}
