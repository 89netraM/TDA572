using System;

namespace Tapper.Model;

public readonly struct Progression
{
	public Func<int, int> BarCount { get; init; } = static bc => bc;
	public Func<double, double> PlayerSpeed { get; init; } = static ps => ps;
	public Func<double, double> GlassSpeed { get; init; } = static gs => gs;
	public Func<int, int> PatronMaxCount { get; init; } = static pmc => Math.Min(pmc + 1, 4);
	public Func<double, double> PatronSpeed { get; init; } = static ps => ps * 1.2d;
	public Func<double, double> PatronBackSpeed { get; init; } = static pbs => pbs;
	public Func<double, double> PatronBackSlide { get; init; } = static pbs => pbs * 0.9d;
	public Func<double, double> PatronTipChance { get; init; } = static ptc => ptc;
}
