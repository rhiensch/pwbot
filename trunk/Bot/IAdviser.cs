using Moves = System.Collections.Generic.List<Bot.Move>;

namespace Bot
{
	public interface IAdviser
	{
		PlanetWars Context { get; set; }
		Moves Run();
	}
}
