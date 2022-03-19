using Microsoft.Extensions.DependencyInjection;
using TSS;
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
game.GameManager.Create<StartMenu>(new StartMenuArgument());
game.Run();
