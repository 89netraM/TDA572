using System;

namespace Tapper.Model
{
	public readonly struct Progression
	{
		public static Progression Default { get; } = new Progression
		{
			BarCount = static bc => bc,
			PlayerSpeed = static ps => ps,
			GlassSpeed = static gs => gs,
			PatronMaxCount = static pmc => Math.Min(pmc + 1, 4),
			PatronSpeed = static ps => ps * 1.2d,
			PatronBackSpeed = static pbs => pbs,
			PatronBackSlide = static pbs => pbs * 0.9d,
			PatronTipChance = static ptc => ptc,
			PatronSpawnChance = static psc => psc,
		};

		public Func<int, int> BarCount { get; init; }
		public Func<double, double> PlayerSpeed { get; init; }
		public Func<double, double> GlassSpeed { get; init; }
		public Func<int, int> PatronMaxCount { get; init; }
		public Func<double, double> PatronSpeed { get; init; }
		public Func<double, double> PatronBackSpeed { get; init; }
		public Func<double, double> PatronBackSlide { get; init; }
		public Func<double, double> PatronTipChance { get; init; }
		public Func<double, double> PatronSpawnChance { get; init; }
	}
}
