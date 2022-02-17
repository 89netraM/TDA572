using Microsoft.Extensions.DependencyInjection;

namespace Zarya.SkiaSharp;

public static class GameBuilderExtensions
{
	/// <summary>
	/// Adds the SkiaSharp backed render system to the game.
	/// </summary>
	public static GameBuilder AddSkiaSharpRenderer(this GameBuilder gameBuilder)
	{
		gameBuilder.Services.AddSingleton<SkiaSharpRenderer>();
		gameBuilder.Services.AddScoped<SkiaSharpSprite>();
		return gameBuilder;
	}
}
