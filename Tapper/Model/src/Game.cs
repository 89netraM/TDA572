using System;
using System.Collections.Generic;

namespace Tapper.Model
{
	public class Game
	{
		public const double BarStart = 0.0d;
		public const double BarEnd = 1.0d;

		public Progression Progression { get; }
		public Settings Settings { get; }

		private readonly Random random;

		private readonly IReadOnlyList<List<Patron>> barPatronPositions;
		public IReadOnlyList<IReadOnlyList<Patron>> BarPateronPositions => barPatronPositions;

		private readonly IReadOnlyList<List<Glass>> barGlassPositions;
		public IReadOnlyList<IReadOnlyList<Glass>> BarGlassesPositions => barGlassPositions;

		public int PlayerBar { get; private set; } = 0;
		public double PlayerBarPosition { get; private set; } = BarEnd;

		public Game() : this(Progression.Default, Settings.Default) { }

		public Game(Progression progression, Settings settings)
		{
			Progression = progression;
			Settings = settings;

			random = Settings.Seed is int seed ? new Random(seed) : new Random();

			var bpp = new List<Patron>[Settings.BarCount];
			for (int b = 0; b < Settings.BarCount; b++)
			{
				bpp[b] = new List<Patron>();
			}
			barPatronPositions = bpp;

			var bgp = new List<Glass>[Settings.BarCount];
			for (int b = 0; b < Settings.BarCount; b++)
			{
				bgp[b] = new List<Glass>();
			}
			barGlassPositions = bgp;
		}

		private int NextId() =>
			random.Next();

		public void Tick(PlayerAction playerAction)
		{
			// Move all patrons
			foreach (var bar in barPatronPositions)
			{
				for (int i = 0; i < bar.Count; i++)
				{
					bar[i] = bar[i].Tick(Settings);
					if (bar[i].Position < BarStart)
					{
						bar.RemoveAt(i);
						i--;
					}
				}
				bar.Sort(Patron.PositionComparer.Default);
			}
			// Move all glasses
			foreach (var bar in barGlassPositions)
			{
				for (int i = 0; i < bar.Count; i++)
				{
					bar[i] = bar[i].Tick(Settings);
					if (bar[i].Position < BarStart)
					{
						bar.RemoveAt(i);
						i--;
					}
				}
				bar.Sort(Glass.PositionComparer.Default);
			}
			// Pick up glasses
			for (int b = 0; b < Settings.BarCount; b++)
			{
				for (int g = 0; g < barGlassPositions[b].Count; g++)
				{
					for (int p = barPatronPositions[b].Count - 1; p >= 0; p--)
					{
						var newPatron = barPatronPositions[b][p].PassGlass(Settings, barGlassPositions[b][g]);
						if (newPatron is not null)
						{
							barPatronPositions[b][p] = newPatron;
							barGlassPositions[b].RemoveAt(g);
							g--;
							break;
						}
					}
				}
			}

			TakePlayerAction(playerAction);
		}
		private void TakePlayerAction(PlayerAction playerAction)
		{
			switch (playerAction)
			{
				case PlayerAction.Left:
					PlayerBarPosition -= Settings.PlayerSpeed;
					break;
				case PlayerAction.Right:
					PlayerBarPosition += Settings.PlayerSpeed;
					break;
				case PlayerAction.Up:
					PlayerBar = ((PlayerBar - 1) % Settings.BarCount + Settings.BarCount) % Settings.BarCount;
					break;
				case PlayerAction.Down:
					PlayerBar = (PlayerBar + 1) % Settings.BarCount;
					break;
				case PlayerAction.Tap:
					PlayerBarPosition = BarEnd;
					barGlassPositions[PlayerBar].Add(new Glass(NextId(), BarEnd));
					break;
				default:
				case PlayerAction.None:
					break;
			}
		}

		public Game NextLevel() =>
			new Game(Progression, Settings.NextLevel(Progression));
	}
}
