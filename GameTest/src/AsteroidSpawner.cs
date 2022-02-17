using System;
using System.Numerics;
using Zarya;

namespace GameTest;

class AsteroidSpawner : IDisposable
{
	private readonly IInputManager inputManager;
	private readonly IGameManager gameManager;

	public AsteroidSpawner(IInputManager inputManager, IGameManager gameManager)
	{
		this.inputManager = inputManager;
		this.gameManager = gameManager;

		gameManager.Update += OnUpdate;
	}

	private void OnUpdate(float deltaTime)
	{
		if (inputManager.IsMouseButtonDown(MouseButton.Left) &&
			inputManager.GetMousePosition() is Vector2 mousePos)
		{
			var asteroid = gameManager.Create<Asteroid>()!;
			asteroid.Transform.Position = mousePos;
		}
	}

	public void Dispose()
	{
		gameManager.Update -= OnUpdate;
	}
}
