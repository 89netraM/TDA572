using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

namespace GameTest;

class Asteroid : IDisposable
{
	private const float Size = 30.0f;

	public Transform2D Transform { get; }
	private readonly PhysicsBody2D physicsBody;
	private readonly IMessageReceiver messageReceiver;
	private readonly TagManager tagManager;

	private readonly SkiaSharpSprite sprite;
	private readonly SilkWindow silkWindow;
	private readonly IInputManager inputManager;
	private readonly IGameManager gameManager;

	private int torqueCounter = 0;

	public Asteroid(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		IMessageReceiver messageReceiver,
		TagManager tagManager,
		SkiaSharpSprite sprite,
		SilkWindow silkWindow,
		IInputManager inputManager,
		IGameManager gameManager)
	{
		Transform = transform;

		this.physicsBody = physicsBody;
		this.physicsBody.Mass = 1.0f;
		this.physicsBody.MaxAngularVelocity = 100.0f;
		this.physicsBody.AngularDrag = 0.1f;
		this.physicsBody.Drag = 0.6f;

		this.messageReceiver = messageReceiver;
		this.messageReceiver.Message += OnMessage;

		this.tagManager = tagManager;
		this.tagManager.AddTag("Target");

		this.sprite = sprite;
		this.sprite.Sprite = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "assets", "asteroid.png");

		this.silkWindow = silkWindow;

		this.inputManager = inputManager;

		this.gameManager = gameManager;
		this.gameManager.Update += OnUpdate;

		this.physicsBody.AddCollider<DebugCircleCollider>(Size, Vector2.Zero);
	}

	private void OnUpdate(float deltaTime)
	{
		if (Transform.Position.X < 0.0f || silkWindow.Width < Transform.Position.X ||
			Transform.Position.Y < 0.0f || silkWindow.Height < Transform.Position.Y)
		{
			gameManager.Destroy(this);
			return;
		}

		if (inputManager.IsMouseButtonDown(MouseButton.Middle) &&
			inputManager.GetMousePosition() is Vector2 mousePos &&
			physicsBody.Colliders.Any(c => c.CheckCollision(mousePos) is not null))
		{
			torqueCounter += 10;
		}

		for (int i = 0; i < torqueCounter; i++)
		{
			physicsBody.AngularVelocity += 0.1f;
		}

		if (torqueCounter > 0)
		{
			torqueCounter--;
		}
	}

	private void OnMessage(object message)
	{
		if (message is BulletHit)
		{
			gameManager.Destroy(this);
		}
	}

	public void Dispose()
	{
		gameManager.Update -= OnUpdate;
		messageReceiver.Message -= OnMessage;
	}
}
