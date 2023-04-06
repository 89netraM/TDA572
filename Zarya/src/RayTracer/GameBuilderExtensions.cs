using Microsoft.Extensions.DependencyInjection;

namespace Zarya.RayTracer;

public static class GameBuilderExtensions
{
	/// <summary>
	/// Adds the Ray Tracer backed render system to the game.
	/// </summary>
	public static GameBuilder AddRayTracerRenderer(this GameBuilder gameBuilder)
	{
		gameBuilder.Services.AddSingleton<RayTracerRenderer>();
		return gameBuilder;
	}
}
