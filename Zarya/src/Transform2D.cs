using System;
using System.Numerics;

namespace Zarya;

/// <summary>
/// The <see cref="Transform2D.Position">position</see> and <see cref="Transform2D.Rotation">rotation</see> of a body in 2D space.
/// </summary>
public class Transform2D
{
	/// <summary>
	/// The position of the body.
	/// </summary>
	public Vector2 Position { get; set; } = Vector2.Zero;

	/// <summary>
	/// The rotation of the body in radians. The rotation is counter-clockwise, starting from the positive x-axis.
	/// </summary>
	public float Rotation { get; set; } = 0.0f;

	/// <summary>
	/// A vector pointing in the direction the body is facing.
	/// </summary>
	public Vector2 Forward => new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
}
