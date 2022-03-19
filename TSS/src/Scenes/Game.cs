using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Zarya;
using Zarya.Silk.NET;

namespace TSS;

class Game : IDisposable
{
	private const float OddsOfIncreasingEnemyCount = 0.05f;
	private const float OddsOfDecreasingEnemySpawnInterval = 0.4f;
	private const float EnemySpawnIntervalDecrease = 0.95f;

	private readonly IGameManager gameManager;
	private readonly PhysicsManager2D physicsManager;
	private readonly SilkWindow window;

	private readonly IList<Player> players = new List<Player>();
	private readonly IList<Enemy> enemies = new List<Enemy>();

	private readonly ScoreRender scoreRender;

	private float EnemySpawnInterval = 2.5f;
	private int EnemyCount = 4;
	private float NextEnemySpawnTime = 0.0f;

	private bool isDisposing = false;

	public Game(
		IGameManager gameManager,
		SilkWindow window,
		PhysicsManager2D physicsManager
	)
	{
		this.gameManager = gameManager;
		this.window = window;

		this.scoreRender = this.gameManager.Create<ScoreRender>()!;

		this.gameManager.Initialize += OnInitialize;

		this.physicsManager = physicsManager;
		this.physicsManager.BeforeUpdate += OnBeforeUpdate;
	}

	private void OnInitialize()
	{
		var player = gameManager.Create<Player>(new Vector2(200.0f, 200.0f))!;
		player.Died += OnPlayerDied;
		players.Add(player);
	}

	private void OnBeforeUpdate()
	{
		if (NextEnemySpawnTime < gameManager.Time && enemies.Count < EnemyCount)
		{
			var spawnPosition = Random.Shared.Next(4) switch
			{
				0 => new Vector2(Enemy.Size + Random.Shared.NextSingle() * ((window.Width ?? 800.0f) - Enemy.Size * 2), Enemy.Size),
				1 => new Vector2(Enemy.Size, Enemy.Size + Random.Shared.NextSingle() * ((window.Height ?? 800.0f) - Enemy.Size * 2)),
				2 => new Vector2(Enemy.Size + Random.Shared.NextSingle() * ((window.Width ?? 800.0f) - Enemy.Size * 2), (window.Height ?? 800.0f) - Enemy.Size),
				3 => new Vector2((window.Width ?? 800.0f) - Enemy.Size, Enemy.Size + Random.Shared.NextSingle() * ((window.Height ?? 800.0f) - Enemy.Size * 2)),
				_ => throw new InvalidOperationException(),
			};

			var enemy = gameManager.Create<Enemy>(spawnPosition)!;
			enemy.Died += OnEnemyDied;
			enemies.Add(enemy);

			NextEnemySpawnTime = gameManager.Time + EnemySpawnInterval;

			if (Random.Shared.NextSingle() < OddsOfIncreasingEnemyCount)
			{
				EnemyCount++;
			}
			if (Random.Shared.NextSingle() < OddsOfDecreasingEnemySpawnInterval)
			{
				EnemySpawnInterval *= EnemySpawnIntervalDecrease;
			}
		}

		var playerPositions = players.Select(p => p.Transform.Position);
		foreach (var enemy in enemies)
		{
			enemy.Update(playerPositions, physicsManager.DeltaTime);
		}
	}

	private void OnPlayerDied(Player player)
	{
		players.Remove(player);
		player.Died -= OnPlayerDied;

		if (!isDisposing && players.Count == 0)
		{
			gameManager.Destroy(this);
			gameManager.Create<StartMenu>(new StartMenuArgument(scoreRender.Score));
		}
	}

	private void OnEnemyDied(Enemy enemy)
	{
		enemies.Remove(enemy);
		enemy.Died -= OnEnemyDied;
		scoreRender.Score++;
	}

	public void Dispose()
	{
		if (isDisposing)
		{
			return;
		}
		isDisposing = true;

		gameManager.Initialize -= OnInitialize;
		physicsManager.BeforeUpdate -= OnBeforeUpdate;

		gameManager.Destroy(scoreRender);

		while (players.Count > 0)
		{
			gameManager.Destroy(players[0]);
		}

		while (enemies.Count > 0)
		{
			gameManager.Destroy(enemies[0]);
		}
	}
}
