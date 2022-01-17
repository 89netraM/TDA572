using System.Collections.Generic;

namespace Tapper.Model;

public class Glass
{
	public int Id { get; }
	public double Position { get; }

	public Glass(int id, double position) =>
		(Id, Position) = (id, position);

	public Glass Tick(Settings settings) =>
		new Glass(Id, Position - settings.GlassSpeed);

	public class PositionComparer : IComparer<Glass>
	{
		public static PositionComparer Default { get; } = new PositionComparer();

		public int Compare(Glass? x, Glass? y) =>
			x is null ? -1 : y is null ? 1 : x.Position.CompareTo(y.Position);
	}
}
