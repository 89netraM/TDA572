using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zarya;

/// <summary>
/// A builder of <see cref="Game{}"/>s.
/// </summary>
public class GameBuilder
{
	/// <summary>
	/// Instantiates a new <see cref="GameBuilder"/>.
	/// </summary>
	public static GameBuilder Create() =>
		new GameBuilder();

	/// <summary>
	/// The services the game will use.
	/// </summary>
	public IServiceCollection Services { get; } = new ServiceCollection();

	private GameBuilder() { }

	/// <summary>
	/// Builds the game and returns it. Without any specific game manager.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// When the <see cref="Services"/> collection does not contain a service of type <see cref="IGameManager"/>.
	/// </exception>
	public Game<IGameManager> Build() =>
		Build<IGameManager>();

	/// <summary>
	/// Builds the game and returns it. The game will hold a reference to the provided <typeparamref name="TGameManager"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// When the <see cref="Services"/> collection does not contain a service of type <see cref="TGameManager"/>.
	/// </exception>
	public Game<TGameManager> Build<TGameManager>() where TGameManager : IGameManager
	{
		var serviceProvider = Services.BuildServiceProvider();
		var gameManager = serviceProvider.GetService<TGameManager>();
		if (gameManager is TGameManager)
		{
			return new Game<TGameManager>(gameManager);
		}
		else
		{
			throw new InvalidOperationException($"No {nameof(IGameManager)} of type {typeof(TGameManager).Name} in {nameof(Services)}.");
		}
	}
}
