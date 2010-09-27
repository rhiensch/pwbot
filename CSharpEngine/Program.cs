using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CSharpEngine
{
	class PlanetWars
	{
		public List<Fleet> fleets;
		public List<Planet> planets;

		public PlanetWars(List<Fleet> inFs, List<Planet> inPs)
		{
			fleets = inFs;
			planets = inPs;
		}
	}

	struct Outcome
	{
		public int winner;
		public List<int> timeout;
		public List<int> fail;
		public string playback;


		public Outcome(string inMessage)
		{
			winner = 0;
			timeout = new List<int>();
			fail = new List<int>();

			playback = inMessage;
		}

		public override string ToString()
		{
			if (winner == 1) return "Player 1 Wins!";
			if (winner == 2) return "Player 2 Wins!";
			else return "Draw!";
		}

	}

	class Order
	{
		public int source;
		public int destination;
		public int num_ships;
		public bool IsNone;

		public static Order None
		{
			get
			{
				Order returnOrder = new Order();
				returnOrder.IsNone = true;
				return returnOrder;
			}
		}
	}

	class Planet
	{
		public double x;
		public double y;
		public int owner;
		public int num_ships;
		public int growth_rate;

		public Planet(double inX, double inY, int inOwner, int inNum_ships, int inGrowth_rate)
		{
			x = inX;
			y = inY;
			owner = inOwner;
			num_ships = inNum_ships;
			growth_rate = inGrowth_rate;
		}
	}

	class Fleet
	{
		public int owner;
		public int num_ships;
		public int source;
		public int destination;
		public int total_trip_length;
		public int turns_remaining;
		public Fleet()
		{
		}

		public Fleet(int inOwner, int inNum_ships, int inSource, int inDestination, int inTotal_trip_length, int inTurns_remaining)
		{
			owner = inOwner;
			num_ships = inNum_ships;
			source = inSource;
			destination = inDestination;
			total_trip_length = inTotal_trip_length;
			turns_remaining = inTurns_remaining;
		}
	}

	static class Program
	{


		// Reads a text file that contains a game state, and returns the game state. A
		// game state is composed of a list of fleets and a list of planets.
		//  map: an absolute or relative filename that point to a text file.
		public static PlanetWars read_map_file(string map)
		{
			List<Fleet> fleets = new List<Fleet>();
			List<Planet> planets = new List<Planet>();
			string mapString = (new StreamReader(map)).ReadToEnd();

			string[] lines = mapString.Trim().Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


			foreach (string eachLine in lines)
			{
				string[] pData = eachLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (pData[0].ToLower() == "p")
				{
					double x = double.Parse(pData[1]);
					double y = double.Parse(pData[2]);
					int owner = int.Parse(pData[3]);
					int num_ships = int.Parse(pData[4]);
					int growth_rate = int.Parse(pData[5]);
					planets.Add(new Planet(x, y, owner, num_ships, growth_rate));
				}
				if (pData[0].ToLower() == "f")
				{
					int owner = int.Parse(pData[1]);
					int num_ships = int.Parse(pData[2]);
					int source = int.Parse(pData[3]);
					int destination = int.Parse(pData[4]);
					int total_trip_length = int.Parse(pData[5]);
					int turns_remaining = int.Parse(pData[6]);

					fleets.Add(new Fleet(owner, num_ships, source, destination, total_trip_length, turns_remaining));
				}
			}

			return new PlanetWars(fleets, planets);
		}

		/*
		# Carries out the point-of-view switch operation, so that each player can
		# always assume that he is player number 1. There are three cases.
		# 1. If pov < 0 then no pov switching is being used. Return player_id.
		# 2. If player_id == pov then return 1 so that each player thinks he is
		#    player number 1.
		# 3. If player_id == 1 then return pov so that the real player 1 looks like
		#    he is player number "pov".
		# 4. Otherwise return player_id, since players other than 1 and pov are
		#    unaffected by the pov switch.
		 */
		public static int switch_pov(int player_id, int pov)
		{
			if (pov < 0) return player_id;
			if (player_id == pov) return 1;
			if (player_id == 1) return pov;
			return player_id;
		}

		//# Generates a string representation of a planet. This is used to send data
		//# about the planets to the client programs.
		public static string serialize_planet(Planet p, int pov)
		{
			int owner = switch_pov(p.owner, pov);
			string message = "P " + p.x + " " + p.y + " " + owner +
			  " " + p.num_ships + " " + p.growth_rate;
			return message.Replace(".0 ", " ");
		}

		//# Generates a string representation of a fleet. This is used to send data
		//# about the fleets to the client programs.
		public static string serialize_fleet(Fleet f, int pov)
		{
			int owner = switch_pov(f.owner, pov);
			string message = "F " + owner + " " + f.num_ships + " " +
			  f.source + " " + f.destination + " " +
			  f.total_trip_length + " " + f.turns_remaining;
			return message.Replace(".0 ", " ");
		}

		//# Takes a string which contains an order and parses it, returning the order in
		//# dictionary format. If the order can't be parsed, return None.
		public static Order parse_order_string(string s)
		{
			string[] tokens = s.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length != 3) return Order.None;
			Order newOrder = new Order();
			if (!int.TryParse(tokens[0], out newOrder.source)) return Order.None;
			if (!int.TryParse(tokens[1], out newOrder.destination)) return Order.None;
			if (!int.TryParse(tokens[2], out newOrder.num_ships)) return Order.None;

			return newOrder;
		}

		//# Calculates the travel time between two planets. This is the cartesian
		//# distance, rounded up to the nearest integer.
		public static int travel_time(Planet aP, Planet bP)
		{
			double dx = bP.x - aP.x;
			double dy = bP.y - aP.y;
			return (int)Math.Ceiling(Math.Sqrt(dx * dx + dy * dy));
		}


		//# Processes the given order, as if it was given by the given player_id. If
		//# everything goes well, returns true. Otherwise, returns false.
		public static bool issue_order(Order order, int player_id, List<Planet> planets, List<Fleet> fleets, ref int[][] temp_fleets)
		{
			if (temp_fleets == null || temp_fleets.Length == 0) temp_fleets = new int[planets.Count][];
			int src = order.source;
			int dest = order.destination;
			if (src < 0 || src >= planets.Count) return false;
			if (dest < 0 || dest >= planets.Count) return false;
			Planet source_planet = planets[src];
			int owner = source_planet.owner;
			int num_ships = order.num_ships;
			if (owner != player_id) return false;

			if (num_ships > source_planet.num_ships) return false;
			if (num_ships < 0) return false;
			source_planet.num_ships -= num_ships;

			if (temp_fleets[src] == null) temp_fleets[src] = new int[planets.Count];
			temp_fleets[src][dest] += num_ships;

			return true;
		}

		//# Processes fleets launched this turn into the normal
		//# fleets array.
		public static void process_new_fleets(List<Planet> planets, ref List<Fleet> fleets, ref int[][] temp_fleets)
		{
			for (int iSource = 0; iSource < temp_fleets.Length; iSource++)
			{
				Planet source_planet = planets[iSource];
				int owner = source_planet.owner;
				if (owner == 0) continue;
				if (temp_fleets[iSource] == null) continue;
				for (int iDest = 0; iDest < temp_fleets[iSource].Length; iDest++)
				{
					int num_ships = temp_fleets[iSource][iDest];
					if (num_ships > 0)
					{
						Planet destination_planet = planets[iDest];
						int t = travel_time(source_planet, destination_planet);
						fleets.Add(new Fleet(owner, num_ships, iSource, iDest, t, t));
					}
				}
			}


		}

		//# "a" is an array. This method returns the number of non-zero elements in a.
		public static int num_non_zero(int[] a)
		{
			int count = 0;
			foreach (int x in a)
			{
				if (x != 0) count++;
			}
			return count;
		}


		public static List<int> addToArray(int index, int value, List<int> inArray)
		{
			if (index >= inArray.Count)
			{
				for (int i = inArray.Count; i <= index; i++)
				{
					inArray.Add(0);					
				}
			}
			inArray[index] += value;
			return inArray;
		}

		public static void cleanUpFleets(ref List<Fleet> fleets)
		{
			List<Fleet> returnList = new List<Fleet>();
			for (int i = 0; i < fleets.Count; i++)
			{
				if (fleets[i] != null) returnList.Add(fleets[i]);
			}
			fleets = returnList;
		}

		//# Resolves the battle at planet p, if there is one.
		//# * removes all fleets involved in the battle
		//# * sets the number of ships and owner of the planet according the outcome
		public static void fight_battle(int pid, ref Planet p, ref List<Fleet> fleets)
		{
			List<int> participants = new List<int>();

			participants = addToArray(p.owner, p.num_ships, participants);
				
			for (int i = fleets.Count - 1; i > -1; i--)
			{
				Fleet f = fleets[i];
				int owner = f.owner;
				if (f.turns_remaining <= 0 && f.destination == pid)
				{
					participants = addToArray(f.owner, f.num_ships, participants);
					fleets[i] = null;

				}

			}
			cleanUpFleets(ref fleets);

			

			Fleet winner = new Fleet();
			Fleet second = new Fleet();

			for (int iOwner = 0; iOwner < participants.Count; iOwner++)
			{
				int ships = participants[iOwner];
				if (ships >= second.num_ships)
				{
					if (ships >= winner.num_ships)
					{
						second.owner = winner.owner;
						second.num_ships = winner.num_ships;

						winner.owner = iOwner;
						winner.num_ships = ships;
					}
					else
					{
						second.owner = iOwner;
						second.num_ships = ships;
					}
				}
			}
			if (winner.num_ships > second.num_ships)
			{
				p.num_ships = winner.num_ships - second.num_ships;
				p.owner = winner.owner;
			}
			else p.num_ships = 0;
		}

		//# Performs the logic needed to advance the state of the game by one turn.
		//# Fleets move forward one tick. Any fleets reaching their destinations are
		//# dealt with. If there are any battles to be resolved, then they're taken
		//# care of.
		public static void do_time_step(ref List<Planet> planets, ref List<Fleet> fleets)
		{
			foreach (Planet p in planets)
			{
				if (p.owner > 0) p.num_ships += p.growth_rate;
			}

			foreach (Fleet f in fleets) f.turns_remaining--;

			for (int i = 0; i < planets.Count; i++)
			{
				Planet refPlanet = planets[i];
				fight_battle(i, ref refPlanet, ref fleets);

			}
		}



		//# Calculates the number of players remaining

		public static List<int> remaining_players(List<Planet> planets, List<Fleet> fleets)
		{
			List<int> players = new List<int>();
			foreach (Planet p in planets)
			{
				if (!players.Contains(p.owner)) players.Add(p.owner);
			}

			foreach (Fleet f in fleets) if (!players.Contains(f.owner)) players.Add(f.owner);
			
			players.Remove(0);
			return players;
		}

		//# Returns a string representation of the entire game state.
		public static string serialize_game_state(List<Planet> planets, List<Fleet> fleets, int pov)
		{
			string message = "";
			foreach (Planet p in planets)
			{
				message += serialize_planet(p, pov) + "\n";
			}
			message += "\n";

			foreach (Fleet f in fleets)	message += serialize_fleet(f, pov) + "\n";

			message += "\ngo\n";
			message = message.Replace("\n\n", "\n");
			return message;
		}

		//# Turns a list of planets into a string in playback format. This is the initial
		//# game state part of a game playback string.
		public static string planet_to_playback_format(List<Planet> planets)
		{
			string planet_string = "";
			foreach (Planet p in planets)
			{
				planet_string += string.Format("{0:0.0##########}", p.x) + "," + string.Format("{0:0.0##########}", p.y) + ","
								+ p.owner + "," + p.num_ships + ","
								+ p.growth_rate + ":";
			}
			planet_string = planet_string.TrimEnd(':');
			return planet_string;
		}


		//# Returns true if(and only if(all the elements of list are true. Otherwise
		//# return false.
		public static bool all_true(bool[] list)
		{
			foreach (bool item in list) if (!item) return false;
			return true;
		}

		//# Kicks the given player from the game. All their fleets disappear. All their
		//# planets become neutral, and keep the same number of ships.
		public static void kick_player_from_game(int player_number, List<Planet> planets, List<Fleet> fleets)
		{
			foreach (Fleet f in fleets)
			{
				if (f.owner == player_number)
				{
					f.turns_remaining = 1;
					f.owner = 0;
					f.num_ships = 0;
				}
			}
			foreach (Planet p in planets)
			{
				if (p.owner == player_number)
				{
					p.owner = 0;
				}
			}
		}

		//# Turns a list of planets into a string in playback format. This is the initial
		//# game state part of a game playback string.
		public static string fleets_to_playback_format(List<Fleet> fleets)
		{
			string fleet_string = "";
			foreach (Fleet f in fleets)
			{
				fleet_string += f.owner + "." + f.num_ships + "." +
								f.source + "." + f.destination + "." +
								f.total_trip_length + "." + f.turns_remaining + ",";
			}
			fleet_string = fleet_string.TrimEnd(',');
			return fleet_string;
		}

		//# Represents the game state in frame format. Represents one frame.
		public static string frame_representation(List<Planet> planets, List<Fleet> fleets)
		{
			string planet_string = "";
			foreach (Planet p in planets) planet_string += p.owner + "." + p.num_ships + ",";
			planet_string += fleets_to_playback_format(fleets);
			planet_string = planet_string.TrimEnd(',');
			return planet_string;
		}


		public static int num_ships_for_player(List<Planet> planets, List<Fleet> fleets, int player_id)
		{
			int total = 0;
			foreach (Planet p in planets) if (p.owner == player_id) total += p.num_ships;
			foreach (Fleet f in fleets) if (f.owner == player_id) total += f.num_ships;
			return total;
		}

		public static int player_with_most_ships(List<Planet> planets, List<Fleet> fleets)
		{

			int max_player = 0;
			int max_ships = 0;
			foreach (int player in remaining_players(planets, fleets))
			{
				int ships = num_ships_for_player(planets, fleets, player);
				if (ships == max_ships) max_player = 0;
				else if (ships > max_ships)
				{
					max_ships = ships;
					max_player = player;
				}
			}
			return max_player;
		}




		public static Process StartClient(string eachPlayer, int playerID)
		{
			bool debug;
#if (DEBUG)
				debug = false;
#else
			debug = false;
#endif
			string[] playerSplit = eachPlayer.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			string playerArgs = "";

			if (!File.Exists(playerSplit[0]))
			{
				Console.Error.Write("\tFailed to start player " + (playerID + 1) + "\n");
				//throw new System.ArgumentException("Invalid player command \"" + playerSplit[0] + "\"");
			}

			for (int i = 1; i < playerSplit.Length; i++)
			{
				playerArgs += playerSplit[i] + " ";
			}
			Process client = new Process();
			client.StartInfo.FileName = playerSplit[0];// eachPlayer;
			client.StartInfo.Arguments = playerArgs;
			client.StartInfo.RedirectStandardOutput = true;
			client.StartInfo.RedirectStandardInput = true;
			client.StartInfo.UseShellExecute = false;
			client.Start();
			if (!client.HasExited)
			{
				if (debug) Console.Error.Write("    started player " + (playerID + 1) + "\n");
			}

			else
			{

				
				Console.Error.Write("    failed to start player " + (playerID + 1) + "\n");
			}


			return client;
		}


		/*
		# Plays a game of Planet Wars.
		#   map: a full or relative path to a text file containing the map that this
		#        game should be played on.
		#   max_turn_time: the maximum amount of time each player gets to finish each
		#                  turn. A player forfeits the game if they run over their
		#                  time allocation.
		#   max_turns: the max length of a game in turns. If the game isn't over after
		#              this many turns, the player with the most ships wins.
		#   players: a list of dictionaries, each of which has information about one
		#            of the players. Each dictionary should have the following keys:
		#            path: the path where the player's files are located
		#            command: the command that invokes the player, assuming the given
		#                     path is the current working directory.
		*/
		public static Outcome play_game(string map, double max_turn_time, int max_turns, string[] players)
		{
			bool debug = false;
#if (DEBUG)
			debug = true;
#endif
			PlanetWars pw = read_map_file(map);
			List<Planet> planets = pw.planets;
			List<Fleet> fleets = pw.fleets;

			string playback = planet_to_playback_format(planets) + "|";
			
			List<Process> clients = new List<Process>();

			if (debug) Console.Error.Write("starting client programs\n");
			for (int iPlayer = 0; iPlayer < players.Length; iPlayer++)
			{
				string eachPlayer = players[iPlayer];
				Process client = StartClient(players[iPlayer], iPlayer);
				clients.Add(client);
				if (client.HasExited) return new Outcome("failure_to_start_client");
			}

			if (debug) Console.Error.Write("waiting for players to spin up\n");
			System.Threading.Thread.Sleep(10);
			int turn_number = 1;
			
			List<string> turn_strings = new List<string>();
			Outcome outcome = new Outcome();
			List<int> remaining = remaining_players(planets, fleets);
			while (turn_number <= max_turns && remaining.Count > 1)
			{
				Console.Error.WriteLine("Turn " + turn_number);
				int iClient = 0;
				int[][] temp_fleets = new int[0][];
				for (iClient = 0; iClient < clients.Count; iClient++)
				{
					//if (i+1) not in remaining continue;
					string cMessage = serialize_game_state(planets, fleets, iClient + 1);
					if (debug)
					{
						//Console.Error.Write("engine > player" + (iClient + 1) + ":\n");
						//Console.Error.Write(cMessage);
					}
					clients[iClient].StandardInput.Write(cMessage);
				}

				bool[] client_done = new bool[clients.Count];//[false] * len(clients)
				DateTime start_time = DateTime.Now;
				double time_limit = max_turn_time;

				//# Get orders from players
				while (!all_true(client_done) && (DateTime.Now - start_time).TotalMilliseconds < time_limit)
				{
					
					for (iClient = 0; iClient < clients.Count; iClient++)
					{
						if (client_done[iClient] || clients[iClient].HasExited || remaining.IndexOf(iClient + 1) == -1) continue;
						string line = clients[iClient].StandardOutput.ReadLine();
						if (string.IsNullOrEmpty(line)) continue;
						line = line.Trim().ToLower();
						if (line == "go") client_done[iClient] = true;
						else
						{
							Order order = parse_order_string(line);
							if (order.IsNone)
							{
								Console.Error.Write("player " + (iClient + 1) + " kicked for making " +
								  "an unparseable order: " + line + "\n");
								clients[iClient].Close();
								kick_player_from_game(iClient + 1, planets, fleets);
							}
							else
							{
								if (!issue_order(order, iClient + 1, planets, fleets, ref temp_fleets))
								{
									Console.Error.Write("player " + (iClient + 1) + " bad order: :" + line + "\n");
									clients[iClient].Close();
									kick_player_from_game(iClient + 1, planets, fleets);
								}
								else if (debug) Console.Error.Write("player " + (iClient + 1) + " order: " + line + "\n");
							}
						}
					}
				}
				process_new_fleets(planets, ref fleets, ref temp_fleets);

				//# Kick players that took too long to move.
				for (iClient = 0; iClient < clients.Count; iClient++)
				{
					if (remaining.IndexOf(iClient + 1) == -1 || client_done[iClient]) continue;

					Console.Error.Write("player " + (iClient + 1) + " kicked for taking too " +
					  "long to move\n");
					if (outcome.timeout == null) outcome.timeout = new List<int>();
					outcome.timeout.Add(iClient + 1);
					kick_player_from_game(iClient + 1, planets, fleets);
				}
				do_time_step(ref planets, ref fleets);
				turn_strings.Add(frame_representation(planets, fleets));
				remaining = remaining_players(planets, fleets);
				turn_number++;

			}

			for (int iClient = 0; iClient < clients.Count; iClient++)
			{
				
				if (clients[iClient] == null || clients[iClient].HasExited)
				{
					if (outcome.fail == null) outcome.fail = new List<int>();
					outcome.fail.Add(iClient + 1);
				}
				clients[iClient].Close();

			}

			foreach (string eTurnString in turn_strings) playback += eTurnString + ":";
			playback = playback.TrimEnd(':');

			outcome.winner = player_with_most_ships(planets, fleets);
			outcome.playback = playback;

			return outcome;


		}

		static void Main(string[] args)
		{
			string map;
			string logFile;
			double max_turn_time;
			int max_turns;
			string[] players;

			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;
			
			// Args: 0]map 1]max_turn_time 2]max_turns 3]logfile 4]bot1 5]bot2
			// logfile is ignored for now
			if (args.Length < 6)
			{
				Console.WriteLine("Needed 6 arguments, got " + args.Length + "!");
		
				Console.WriteLine("<string:map> <double:max turn time> <int:max turns> <string:log file> <string:bot 1 exec> <string:bot 2 exec>");
				return;
			}
			map = args[0];
			bool valid_max_turn_time = double.TryParse(args[1], out max_turn_time);
			bool valid_max_turns = int.TryParse(args[2], out max_turns);
			logFile = args[3];
			players = new string[2] { args[4], args[5] };

			if (map == "" || !File.Exists(map))
			{
				throw new System.ArgumentException("Invalid map or map not found \"" + args[0] + "\"", "map");
			}

			if (!valid_max_turn_time || max_turn_time <= 0.0)
			{
				throw new System.ArgumentException("Invalid max turn time \"" + args[1] + "\"", "max_turn_time");
			}

			if (!valid_max_turns || max_turns <= 0)
			{
				throw new System.ArgumentException("Invalid max turns \"" + args[2] + "\"", "max_turns");
			}

			if (players[0] == "" || players[1] == "")
			{
				throw new System.ArgumentException("Invalid player used 1:\"" + args[4] + "\" 2:\"" + args[5] + "\"", "players");
			}

			try
			{
				Outcome newOutcome = play_game(map, max_turn_time, max_turns, players);
				Console.Error.Write(newOutcome.ToString());
				Console.Write(newOutcome.playback);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Play Game Failed!");
				Console.Error.Write(ex.ToString());
			}
						
		}
	}
}
