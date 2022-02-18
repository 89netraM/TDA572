using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

namespace GameTest;

class Spaceship : IDisposable
{
	public Transform2D Transform { get; }
	private readonly PhysicsBody2D physicsBody;

	private readonly SkiaSharpSprite sprite;
	private readonly SilkWindow silkWindow;
	private readonly Input input;
	private readonly PhysicsManager2D physicsManager;
	private readonly IGameManager gameManager;

	public Spaceship(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		SkiaSharpSprite sprite,
		SilkWindow silkWindow,
		Input input,
		PhysicsManager2D physicsManager,
		IGameManager gameManager)
	{
		this.Transform = transform;

		this.physicsBody = physicsBody;
		this.physicsBody.MaxVelocity = 600.0f;
		this.physicsBody.AngularDrag = 0.01f;
		this.physicsBody.Mass = 4.0f;

		this.sprite = sprite;
		this.sprite.Sprite = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "assets", "spaceship.png");

		this.silkWindow = silkWindow;

		this.input = input;

		this.physicsManager = physicsManager;
		this.physicsManager.BeforeUpdate += OnBeforePhysicsUpdate;

		this.gameManager = gameManager;
		this.gameManager.Update += OnUpdate;

		this.physicsBody.AddCollider<DebugCircleCollider>(5.0f, new Vector2(-30.0f, -17.0f));
		this.physicsBody.AddCollider<DebugCircleCollider>(5.0f, new Vector2(-30.0f, 17.0f));
		this.physicsBody.AddCollider<DebugCircleCollider>(5.0f, new Vector2(30.0f, 0.0f));
	}

	private void OnBeforePhysicsUpdate()
	{
		physicsBody.Velocity += input.Vertical() * Transform.Forward * 30.0f;
		physicsBody.AngularVelocity += input.Horizontal() * MathF.PI / 10.0f;
	}

	private void OnUpdate(float deltaTime)
	{
		if (input.Reset())
		{
			Transform.Position = new Vector2(500.0f, 500.0f);
			Transform.Rotation = 0.0f;
			physicsBody.Velocity = Vector2.Zero;
			physicsBody.AngularVelocity = 0.0f;
		}

		if (input.Fire())
		{
			FireBullet();
		}
	}

	private void FireBullet()
	{
		var bullet = gameManager.Create<Bullet>()!;
		bullet.Transform.Position = Transform.Position;
		bullet.Transform.Rotation = Transform.Rotation;
		bullet.PhysicsBody.Velocity = Transform.Forward * 600.0f;
	}

	public void Dispose()
	{
		this.physicsManager.BeforeUpdate -= OnBeforePhysicsUpdate;
		this.gameManager.Update -= OnUpdate;
	}
}
