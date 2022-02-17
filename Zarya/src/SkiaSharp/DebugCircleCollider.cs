using System;
using System.Numerics;
using SkiaSharp;

namespace Zarya.SkiaSharp;

/// <summary>
/// A <see cref="CircleCollider"/> that renders to screen.
/// </summary>
public class DebugCircleCollider : CircleCollider, ISkiaSharpRenderable, IDisposable
{
	private readonly SkiaSharpRenderer renderer;
	private readonly Transform2D transform;
	Transform2D ISkiaSharpRenderable.Transform => transform;
	private readonly PhysicsBody2D physicsBody;
	private SKColor color = SKColors.Green;

	public DebugCircleCollider(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		SkiaSharpRenderer renderer,
		float radius,
		Vector2 offset) : base(transform, radius, offset)
	{
		this.transform = transform;

		this.physicsBody = physicsBody;
		this.physicsBody.CollisionEnter += OnCollisionEnter;
		this.physicsBody.CollisionStay += OnCollisionStay;
		this.physicsBody.CollisionExit += OnCollisionExit;

		this.renderer = renderer;
		this.renderer.AddRenderable(this);
	}

	private void OnCollisionEnter(PhysicsBody2D other, Vector2 impact) =>
		color = SKColors.Red;
	private void OnCollisionStay(PhysicsBody2D other, Vector2 impact) =>
		color = SKColors.Blue;
	private void OnCollisionExit(PhysicsBody2D other) =>
		color = SKColors.Green;

	void ISkiaSharpRenderable.Render(SKCanvas canvas)
	{
		using var paint = new SKPaint { Color = color, IsStroke = true };
		canvas.DrawCircle(Offset.X, Offset.Y, Radius, paint);
	}

	public void Dispose()
	{
		renderer.RemoveRenderable(this);
	}
}
