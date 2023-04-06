using System;
using System.Numerics;
using RayTest;
using Zarya;
using Zarya.Silk.NET;
using Zarya.RayTracer;

var builder = GameBuilder.Create();
builder.AddSilkWindow();
builder.AddRayTracerRenderer();

var game = builder.Build<SilkWindow>();
CreateBall(position: new(-10.0f, 0.0f, 20.0f), albedo: new(1.0f, 0.0f, 0.0f));
CreateBall(position: new(0.0f, 0.0f, 20.0f), waveOffset: MathF.PI / 2.0f, albedo: new(0.0f, 1.0f, 0.0f));
CreateBall(position: new(10.0f, 0.0f, 20.0f), waveOffset: MathF.PI, albedo: new(0.0f, 0.0f, 1.0f));
CreateBall(position: new(12.5f, 10.0f, 17.5f), radius: 5.0f, waveRadius: 0.0f, albedo: new(0.0f, 0.0f, 0.0f), emissive: new(1000.0f, 900.0f, 700.0f));
CreateBall(position: new(0.0f, -1006.5f, 20.0f), radius: 1000.0f, waveRadius: 0.0f);
game.Run();

void CreateBall(
	Vector3? position = null,
	float waveRadius = 5.0f,
	float waveOffset = 0.0f,
	float radius = 1.0f,
	Vector3? albedo = null,
	Vector3? emissive = null)
{
	var ball3 = game.GameManager.Create<Ball>()!;
	ball3.Position = position ?? new(0.0f, 0.0f, 20.0f);
	ball3.WaveRadius = waveRadius;
	ball3.WaveOffset = waveOffset;
	ball3.Radius = radius;
	ball3.Albedo = albedo ?? new(1.0f, 1.0f, 1.0f);
	ball3.Emissive = emissive ?? new(0.0f, 0.0f, 0.0f);
}
