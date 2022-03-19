using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SkiaSharp;
using Zarya;
using Zarya.SkiaSharp;

namespace TSS;

class Enemy : ISkiaSharpRenderable, IDisposable
{
	public const float Size = 48.0f;
	private const float Speed = 150.0f;

	private readonly Transform2D transform;
	Transform2D ISkiaSharpRenderable.Transform => transform;
	private readonly IMessageReceiver messageReceiver;
	private readonly IGameManager gameManager;
	private readonly SkiaSharpRenderer renderer;

	public event Action<Enemy>? Died;

	public Enemy(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		TagManager tagManager,
		IMessageReceiver messageReceiver,
		IGameManager gameManager,
		SkiaSharpRenderer renderer,
		Vector2 startPosition
	)
	{
		this.transform = transform;
		this.transform.Position = startPosition;

		physicsBody.AddCollider<CircleCollider>(Vector2.Zero, Size);

		tagManager.AddTag(Tags.Target);
		tagManager.AddTag(Tags.Enemy);

		this.messageReceiver = messageReceiver;
		this.messageReceiver.Message += OnMessage;

		this.gameManager = gameManager;

		this.renderer = renderer;
		this.renderer.AddRenderable(this);
	}

	public void Update(IEnumerable<Vector2> playerPositions, float deltaTime)
	{
		var closestPlayer = playerPositions.MinBy(p => Vector2.Distance(transform.Position, p));
		var direction = Vector2.Normalize(closestPlayer - transform.Position);
		transform.Position += direction * Speed * deltaTime;
	}

	private void OnMessage(object message)
	{
		if (message is BulletHit)
		{
			gameManager.Destroy(this);
		}
	}

	public void Render(SKCanvas canvas)
	{
		using var paint = new SKPaint { Color = SKColors.OrangeRed };
		canvas.DrawCircle(0.0f, 0.0f, Size, paint);
	}

	public void Dispose()
	{
		messageReceiver.Message -= OnMessage;
		renderer.RemoveRenderable(this);

		Died?.Invoke(this);
	}
}

class EnemyHit
{
	public static EnemyHit Instance { get; } = new EnemyHit();
}
