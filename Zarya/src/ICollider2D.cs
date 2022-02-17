using System;
using System.Numerics;

namespace Zarya;

/// <summary>
/// An interface for different colliders.
/// </summary>
public interface ICollider2D
{
	/// <summary>
	/// Collides this collider with <paramref name="other"/> and returns the impace vector, or null if no collision.
	/// </summary>
	/// <exception cref="NotImplementedException">
	/// When <paramref name="other"/> is of a type this collider does not support.
	/// </exception>
	Vector2? CheckCollision(ICollider2D other)
	{
		if (other is CircleCollider circle)
		{
			return CheckCollision(circle);
		}
		else
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Collides this collider with <paramref name="other"/> circle colider and returns the impace vector, or null if no collision.
	/// </summary>
	Vector2? CheckCollision(CircleCollider other);
	/// <summary>
	/// Collides this collider with the point <paramref name="other"/> and returns the impace vector, or null if no collision.
	/// </summary>
	Vector2? CheckCollision(Vector2 other);

	/// <summary>
	/// Calculates the axis-aligned bounding box of this collider.
	/// </summary>
	AABB2D CalculateAABB();
}
