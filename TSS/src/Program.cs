﻿using TSS;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

var builder = GameBuilder.Create();
builder.AddDefault2DPhysics();
builder.AddMessagePasser();
builder.AddTagManager();
builder.AddSilkWindow();
builder.AddSkiaSharpRenderer();

var game = builder.Build<SilkWindow>();
game.GameManager.Create<StartMenu>(new StartMenuArgument());
game.Run();
