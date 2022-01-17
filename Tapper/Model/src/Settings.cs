namespace Tapper.Model
{
	public readonly struct Settings
	{
		public static Settings Default { get; } = new Settings
		{
			Seed = null,
			BarCount = 4,
			PlayerSpeed = 0.05d,
			GlassSpeed = 0.1d,
			PatronMaxCount = 2,
			PatronSpeed = 0.05d,
			PatronBackSpeed = 0.25d,
			PatronBackSlide = 0.5d,
			PatronTipChance = 0.25d,
		};

		public int? Seed { get; init; }
		public int BarCount { get; init; }
		public double PlayerSpeed { get; init; }
		public double GlassSpeed { get; init; }
		public int PatronMaxCount { get; init; }
		public double PatronSpeed { get; init; }
		public double PatronBackSpeed { get; init; }
		public double PatronBackSlide { get; init; }
		public double PatronTipChance { get; init; }

		public Settings NextLevel(Progression p) =>
			new Settings
			{
				Seed = Seed,
				BarCount = p.BarCount(BarCount),
				PlayerSpeed = p.PlayerSpeed(PlayerSpeed),
				GlassSpeed = p.GlassSpeed(GlassSpeed),
				PatronMaxCount = p.PatronMaxCount(PatronMaxCount),
				PatronSpeed = p.PatronSpeed(PatronSpeed),
				PatronBackSpeed = p.PatronBackSpeed(PatronBackSpeed),
				PatronBackSlide = p.PatronBackSlide(PatronBackSlide),
				PatronTipChance = p.PatronTipChance(PatronTipChance),
			};
	}
}
