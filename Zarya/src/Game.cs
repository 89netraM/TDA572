namespace Zarya;

/// <summary>
/// A game that can be setup through the <see cref="Game{}.GameManager"/>, and then run.
/// </summary>
public class Game<TGameManager> where TGameManager : IGameManager
{
	/// <summary>
	/// The game manager that manages the game.
	/// </summary>
	public TGameManager GameManager { get; }

	internal Game(TGameManager gameManager) =>
		GameManager = gameManager;

	/// <summary>
	/// Runs the game to completion.
	/// </summary>
	public void Run() =>
		GameManager.Run();
}
