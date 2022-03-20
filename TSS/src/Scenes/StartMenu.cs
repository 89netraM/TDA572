using System;
using System.Numerics;
using SkiaSharp;
using Zarya;
using Zarya.Silk.NET;

namespace TSS;

class StartMenu : IDisposable
{
	private readonly IGameManager gameManager;
	private readonly SilkWindow window;

	private Player? player;
	private ButtonTarget? startButton;
	private ButtonTarget? exitButton;

	private ScoreRender? scoreRender;

	public StartMenu(
		IGameManager gameManager,
		SilkWindow window,
		StartMenuArgument arg
	)
	{
		this.gameManager = gameManager;
		this.window = window;

		if (arg.Score is int s)
		{
			this.scoreRender = this.gameManager.Create<ScoreRender>()!;
			this.scoreRender.Score = s;
		}

		this.gameManager.Initialize += OnInitialize;
	}

	private void OnInitialize()
	{
		var center = new Vector2(400.0f, 400.0f);
		if (window.Width is int w && window.Height is int h)
		{
			center = new Vector2(w / 2.0f, h / 2.0f);
		}

		player = gameManager.Create<Player>(center, PlayerHealthPosition.TopLeft, gameManager.Create<AnyInput>()!);

		var buttonOffset = new Vector2(200.0f, 100.0f);

		startButton = gameManager.Create<ButtonTarget>(center - buttonOffset, "Start", SKColors.Green)!;
		startButton.Activate += OnStartButtonActivate;

		exitButton = gameManager.Create<ButtonTarget>(center + buttonOffset, "Exit", SKColors.Red)!;
		exitButton.Activate += OnExitButtonActivate;
	}

	private void OnStartButtonActivate()
	{
		gameManager.Destroy(this);
		gameManager.Create<Game>();
	}

	private void OnExitButtonActivate() =>
		gameManager.Close();

	public void Dispose()
	{
		gameManager.Initialize -= OnInitialize;

		startButton!.Activate -= OnStartButtonActivate;
		gameManager.Destroy(startButton!);
		exitButton!.Activate -= OnExitButtonActivate;
		gameManager.Destroy(exitButton!);

		if (scoreRender is ScoreRender sr)
		{
			gameManager.Destroy(sr);
		}

		gameManager.Destroy(player!);
	}
}

record StartMenuArgument(int? Score = null);
