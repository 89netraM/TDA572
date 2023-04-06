using System;
using System.Numerics;
using Zarya;
using Zarya.RayTracer;

namespace RayTest;

class Ball : IRayTraceRenderable, IDisposable
{
	private readonly IGameManager gameManager;
	private readonly RayTracerRenderer renderer;

	private Vector3 basePosition;
	private Vector3 position;
	public Vector3 Position
	{
		get => position;
		set
		{
			basePosition = value;
			position = value;
		}
	}
	public float WaveRadius { get; set; }
	public float WaveOffset { get; set; }
	public float Radius { get; set; }
	public Vector3 Albedo { get; set; }
	public Vector3 Emissive { get; set; }
	public RayTracerObject TraceableObject => new(Position, Radius, Albedo, Emissive);

	public Ball(IGameManager gameManager, RayTracerRenderer renderer)
	{
		this.gameManager = gameManager;
		this.gameManager.Update += OnUpdate;

		this.renderer = renderer;
		this.renderer.AddRenderable(this);
	}

	private void OnUpdate(float deltaTime)
	{
		position = new(basePosition.X, basePosition.Y + MathF.Sin(gameManager.Time + WaveOffset) * WaveRadius, basePosition.Z);
	}

	public void Dispose()
	{
		renderer.RemoveRenderable(this);
		gameManager.Update -= OnUpdate;
	}
}
