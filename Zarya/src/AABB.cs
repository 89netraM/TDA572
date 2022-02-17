using System;
using System.Numerics;

namespace Zarya;

/// <summary>
/// An axis-aligned bounding box in 2D with a lower and upper corner.
/// </summary>
/// <param name="Min">
/// The lower "left" corner of the AABB.
/// </param>
/// <param name="Max">
/// The upper "right" corner of the AABB.
/// </param>
public record AABB2D(Vector2 Min, Vector2 Max) : IComparable<AABB2D>
{
	int IComparable<AABB2D>.CompareTo(AABB2D? other)
	{
		if (other is null)
		{
			return 1;
		}
		else
		{
			return Min.X.CompareTo(other.Min.X) switch
			{
				0 => Min.Y.CompareTo(other.Min.Y),
				int i => i,
			};
		}
	}
}
