using Microsoft.Extensions.DependencyInjection;

namespace Zarya.Silk.NET;

public static class GameBuilderExtensions
{
	/// <summary>
	/// Adds <see cref="SilkWindow"/> as <see cref="IGameManager"/> and <see cref="IInputManager"/> to the game.
	/// </summary>
	public static GameBuilder AddSilkWindow(this GameBuilder gameBuilder)
	{
		gameBuilder.Services.AddSingleton<SilkWindow>();
		gameBuilder.Services.AddSingleton<IGameManager>(sp => sp.GetRequiredService<SilkWindow>());
		gameBuilder.Services.AddSingleton<IInputManager>(sp => sp.GetRequiredService<SilkWindow>());
		return gameBuilder;
	}
}
