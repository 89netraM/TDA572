using System;
using System.Numerics;
using SkiaSharp;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

namespace GameTest;

class Bullet : ISkiaSharpRenderable, IDisposable
{
	private const float Size = 10.0f;

	public Transform2D Transform { get; }
	public PhysicsBody2D PhysicsBody { get; }

	private readonly SkiaSharpRenderer renderer;
	private readonly SilkWindow silkWindow;
	private readonly IGameManager gameManager;

	public Bullet(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		SkiaSharpRenderer renderer,
		SilkWindow silkWindow,
		IGameManager gameManager)
	{
		Transform = transform;

		PhysicsBody = physicsBody;
		PhysicsBody.IsPassThrough = true;
		PhysicsBody.CollisionEnter += OnCollisionEnter;

		this.renderer = renderer;
		this.renderer.AddRenderable(this);

		this.silkWindow = silkWindow;

		this.gameManager = gameManager;
		this.gameManager.Update += OnUpdate;

		PhysicsBody.AddCollider<DebugCircleCollider>(Size, Vector2.Zero);
	}

	private void OnUpdate(float deltaTime)
	{
		if (Transform.Position.X < 0.0f || silkWindow.Width < Transform.Position.X ||
			Transform.Position.Y < 0.0f || silkWindow.Height < Transform.Position.Y)
		{
			gameManager.Destroy(this);
		}
	}

	private void OnCollisionEnter(PhysicsBody2D other, Vector2 impulse)
	{
		if (other.Tag.HasTag("Target"))
		{
			other.MessagePasser.SendMessage(BulletHit.Instance);
			gameManager.Destroy(this);
		}
	}

	public void Render(SKCanvas canvas)
	{
		using SKPaint red = new SKPaint { Color = SKColors.Red };
		canvas.DrawLine(-Size, -Size, +Size, +Size, red);
		canvas.DrawLine(-Size, +Size, +Size, -Size, red);
	}

	public void Dispose()
	{
		PhysicsBody.CollisionEnter -= OnCollisionEnter;
		renderer.RemoveRenderable(this);
		gameManager.Update -= OnUpdate;
	}
}

class BulletHit
{
	public static BulletHit Instance { get; } = new BulletHit();
}
