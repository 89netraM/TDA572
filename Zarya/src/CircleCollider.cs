using System;
using System.Numerics;

namespace Zarya;

/// <summary>
/// A circular collider.
/// </summary>
public class CircleCollider : ICollider2D
{
	private readonly Transform2D transform;

	/// <summary>
	/// The radius of the collider.
	/// </summary>
	public float Radius { get; }
	/// <summary>
	/// The position of the collider relative to the <see cref="Transform2D"/> of the entity.
	/// </summary>
	public Vector2 Offset { get; }

	public CircleCollider(Transform2D transform, float radius, Vector2 offset)
	{
		this.transform = transform;
		Radius = radius;
		Offset = offset;
	}

	Vector2? ICollider2D.CheckCollision(CircleCollider other)
	{
		float sqrRad = MathF.Pow(Radius + other.Radius, 2);
		var center = ColliderCenter();
		var otherCenter = other.ColliderCenter();
		float sqrDist = Vector2.DistanceSquared(center, otherCenter);
		if (sqrDist <= sqrRad)
		{
			float rad = Radius + other.Radius;
			float dist = MathF.Sqrt(sqrDist);
			return (center - otherCenter) * (rad - dist) / rad;
		}
		else
		{
			return null;
		}
	}

	Vector2? ICollider2D.CheckCollision(Vector2 point)
	{
		float sqrRad = MathF.Pow(Radius, 2);
		var center = ColliderCenter();
		float sqrDist = Vector2.DistanceSquared(center, point);
		if (sqrDist <= sqrRad)
		{
			float rad = Radius;
			float dist = MathF.Sqrt(sqrDist);
			return (center - point) * (rad - dist) / rad;
		}
		else
		{
			return null;
		}
	}

	AABB2D ICollider2D.CalculateAABB()
	{
		var center = ColliderCenter();
		var halfSize = new Vector2(Radius, Radius);
		return new AABB2D(center - halfSize, center + halfSize);
	}

	private Vector2 ColliderCenter() =>
		transform.Position +
			Vector2.Transform(Offset, Matrix3x2.CreateRotation(transform.Rotation));
}
