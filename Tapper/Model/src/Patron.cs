using System.Collections.Generic;

namespace Tapper.Model;

public abstract class Patron
{
	public int Id { get; }
	public double Position { get; }

	public Patron(int id, double position) =>
		(Id, Position) = (id, position);

	public abstract Patron? PassGlass(Settings settings, Glass glass);
	public abstract Patron Tick(Settings settings);

	public class PositionComparer : IComparer<Patron>
	{
		public static PositionComparer Default { get; } = new PositionComparer();

		public int Compare(Patron? x, Patron? y) =>
			x is null ? -1 : y is null ? 1 : x.Position.CompareTo(y.Position);
	}
}

public class ThirstyPatron : Patron
{
	public ThirstyPatron(int id, double position) : base(id, position) { }

	public override Patron? PassGlass(Settings settings, Glass glass) =>
		glass.Position < Position ? new DrinkingPatron(Id, Position) : null;

	public override Patron Tick(Settings settings) =>
		new ThirstyPatron(Id, Position + settings.PatronSpeed);
}

public class DrinkingPatron : Patron
{
	public double GotGlassPosition { get; }

	public DrinkingPatron(int id, double position) : base(id, position) =>
		GotGlassPosition = position;
	public DrinkingPatron(int id, double position, double gotGlassPosition) : base(id, position) =>
		GotGlassPosition = gotGlassPosition;

	public override Patron? PassGlass(Settings settings, Glass glass) =>
		null;

	public override Patron Tick(Settings settings)
	{
		var newPosition = Position - settings.PatronBackSpeed;
		if (newPosition < GotGlassPosition - settings.PatronBackSlide)
		{
			return new ThirstyPatron(Id, GotGlassPosition - settings.PatronBackSlide);
		}
		else
		{
			return new DrinkingPatron(Id, newPosition, GotGlassPosition);
		}
	}
}
