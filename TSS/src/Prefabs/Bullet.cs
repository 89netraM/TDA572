using System;
using System.Numerics;
using SkiaSharp;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

namespace TSS;

class Bullet : ISkiaSharpRenderable, IDisposable
{
	private const float Speed = 1000.0f;
	private const float Size = 10.0f;

	private readonly Transform2D transform;
	private readonly PhysicsBody2D physicsBody;

	private readonly SkiaSharpRenderer renderer;
	private readonly SilkWindow silkWindow;
	private readonly IGameManager gameManager;

	Transform2D ISkiaSharpRenderable.Transform => transform;

	public Bullet(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		SkiaSharpRenderer renderer,
		SilkWindow silkWindow,
		IGameManager gameManager,
		Vector2 startPosition,
		float rotation
	)
	{
		this.transform = transform;
		this.transform.Position = startPosition;
		this.transform.Rotation = rotation;

		this.physicsBody = physicsBody;
		this.physicsBody.IsPassThrough = true;
		this.physicsBody.Velocity = this.transform.Forward * Speed;
		this.physicsBody.CollisionEnter += OnCollisionEnter;
		this.physicsBody.AddCollider<CircleCollider>(Size, Vector2.Zero);

		this.renderer = renderer;
		this.renderer.AddRenderable(this);

		this.silkWindow = silkWindow;

		this.gameManager = gameManager;
		this.gameManager.Update += OnUpdate;
	}

	private void OnUpdate(float deltaTime)
	{
		if (transform.Position.X < 0.0f || silkWindow.Width < transform.Position.X ||
			transform.Position.Y < 0.0f || silkWindow.Height < transform.Position.Y)
		{
			gameManager.Destroy(this);
		}
	}

	void ISkiaSharpRenderable.Render(SKCanvas canvas)
	{
		using var paint = new SKPaint { Color = SKColors.Gray };
		canvas.DrawCircle(0.0f, 0.0f, 10.0f, paint);
	}

	private void OnCollisionEnter(PhysicsBody2D other, Vector2 impulse)
	{
		if (other.Tag.HasTag(Tags.Target))
		{
			other.MessagePasser.SendMessage(BulletHit.Instance);
			gameManager.Destroy(this);
		}
	}

	public void Dispose()
	{
		renderer.RemoveRenderable(this);
		physicsBody.CollisionEnter -= OnCollisionEnter;
		gameManager.Update -= OnUpdate;
	}
}

class BulletHit
{
	public static BulletHit Instance { get; } = new BulletHit();
}
