using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using SkiaSharp;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

namespace TSS;

class Player : ISkiaSharpRenderable, IDisposable
{
	private const float Size = 48.0f;
	private const float Speed = 250.0f;
	private const float EnemyDPS = 0.25f;
	private const float TimeBetweenBullets = 0.25f;

	public Transform2D Transform { get; }
	private readonly PhysicsBody2D physicsBody;
	private readonly PlayerHealth health;
	private readonly SkiaSharpRenderer renderer;
	private readonly SilkWindow window;
	private readonly IGameManager gameManager;
	private readonly PhysicsManager2D physicsManager;
	private readonly Input input;

	public event Action<Player>? Died;

	private readonly SKColor color;

	private float nextBulletTime = 0.0f;

	public Player(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		SkiaSharpRenderer renderer,
		TagManager tagManager,
		SilkWindow window,
		IGameManager gameManager,
		PhysicsManager2D physicsManager,
		Input input,
		Vector2 startPosition
	)
	{
		Transform = transform;
		Transform.Position = startPosition;

		this.physicsBody = physicsBody;
		this.physicsBody.AddCollider<CircleCollider>(Vector2.Zero, Size);
		this.physicsBody.CollisionStay += OnCollisionStay;

		this.renderer = renderer;
		this.renderer.AddRenderable(this);
		color = SKColor.FromHsv(Random.Shared.NextSingle() * 256.0f, 255.0f, 255.0f);

		tagManager.AddTag(Tags.Player);

		this.window = window;

		this.gameManager = gameManager;
		this.gameManager.Update += OnUpdate;

		this.physicsManager = physicsManager;
		this.physicsManager.BeforeUpdate += OnBeforePhysicsUpdate;
		this.physicsManager.AfterUpdate += OnAfterPhysicsUpdate;

		this.health = this.gameManager.Create<PlayerHealth>()!;
		this.health.Color = color;

		this.input = input;
	}

	private void OnUpdate(float deltaTime)
	{
		if (nextBulletTime < gameManager.Time && input.Fire())
		{
			gameManager.Create<Bullet>(Transform.Position + Transform.Forward * 64.0f, Transform.Rotation);
			nextBulletTime = gameManager.Time + TimeBetweenBullets;
		}
	}

	private void OnBeforePhysicsUpdate()
	{
		var moveDir = new Vector2(input.Horizontal(), -input.Vertical());
		if (moveDir.LengthSquared() > 1.0f)
		{
			moveDir = Vector2.Normalize(moveDir);
		}
		Transform.Position += moveDir * Speed * physicsManager.DeltaTime;

		var lookDir = new Vector2(input.LookHorizontal(), -input.LookVertical());
		if (lookDir.LengthSquared() > 0.25f)
		{
			var rotation = MathF.Acos(Vector2.Dot(lookDir, Vector2.UnitX) / lookDir.Length());
			if (lookDir.Y < 0.0f)
			{
				rotation = -rotation;
			}
			Transform.Rotation = rotation;
		}
	}

	private void OnCollisionStay(PhysicsBody2D other, Vector2 impulse)
	{
		if (other.Tag.HasTag(Tags.Enemy))
		{
			health.Health -= EnemyDPS * physicsManager.DeltaTime;
			if (health.Health <= 0.0f)
			{
				gameManager.Destroy(this);
			}
		}
	}

	private void OnAfterPhysicsUpdate()
	{
		if (Transform.Position.X < Size)
		{
			Transform.Position = new Vector2(Size, Transform.Position.Y);
		}
		else if (window.Width is int w && w - Size < Transform.Position.X)
		{
			Transform.Position = new Vector2(w - Size, Transform.Position.Y);
		}
		if (Transform.Position.Y < Size)
		{
			Transform.Position = new Vector2(Transform.Position.X, Size);
		}
		else if (window.Height is int h && h - Size < Transform.Position.Y)
		{
			Transform.Position = new Vector2(Transform.Position.X, h - Size);
		}
	}

	public void Render(SKCanvas canvas)
	{
		using (var paint = new SKPaint { Color = color })
		{
			canvas.DrawCircle(0.0f, 0.0f, Size, paint);
		}
		using (var paint = new SKPaint { Color = new SKColor(160, 160, 160) })
		{
			canvas.DrawRect(14.0f, -6.0f, 50.0f, 12.0f, paint);
		}
	}

	public void Dispose()
	{
		gameManager.Update -= OnUpdate;
		physicsBody.CollisionStay -= OnCollisionStay;
		physicsManager.BeforeUpdate -= OnBeforePhysicsUpdate;
		physicsManager.AfterUpdate -= OnAfterPhysicsUpdate;

		renderer.RemoveRenderable(this);

		Died?.Invoke(this);
	}
}
