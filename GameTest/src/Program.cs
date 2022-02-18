using System.Numerics;
using GameTest;
using Microsoft.Extensions.DependencyInjection;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

var builder = GameBuilder.Create();
builder.AddDefault2DPhysics();
builder.AddMessagePasser();
builder.AddTagManager();
builder.AddSilkWindow();
builder.AddSkiaSharpRenderer();
builder.Services.AddSingleton<Input>();

var game = builder.Build<SilkWindow>();
var spaceship = game.GameManager.Create<Spaceship>()!;
spaceship.Transform.Position = new Vector2(500.0f, 500.0f);
var asteroid = game.GameManager.Create<Asteroid>()!;
asteroid.Transform.Position = new Vector2(600.0f, 500.0f);
game.GameManager.Create<AsteroidSpawner>();
game.Run();
