namespace Tapper.Model;

public readonly struct Settings
{
	public int? Seed { get; init; } = null;
	public int BarCount { get; init; } = 4;
	public double PlayerSpeed { get; init; } = 0.05d;
	public double GlassSpeed { get; init; } = 0.1d;
	public int PatronMaxCount { get; init; } = 2;
	public double PatronSpeed { get; init; } = 0.05d;
	public double PatronBackSpeed { get; init; } = 0.25d;
	public double PatronBackSlide { get; init; } = 0.5d;
	public double PatronTipChance { get; init; } = 0.25d;

	public Settings NextLevel(Progression p) =>
		this with
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
