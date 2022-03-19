using System;
using System.Numerics;
using SkiaSharp;
using Zarya;
using Zarya.SkiaSharp;

namespace TSS;

class ScoreRender : ISkiaSharpRenderable, IDisposable
{
	public Transform2D Transform { get; }
	private readonly SkiaSharpRenderer renderer;

	public int Score { get; set; } = 0;

	public ScoreRender(
		Transform2D transform,
		SkiaSharpRenderer renderer
	)
	{
		Transform = transform;
		transform.Position = new Vector2(PlayerHealth.Margin * 2.0f + PlayerHealth.Width, PlayerHealth.Margin / 2.0f + PlayerHealth.Height);

		this.renderer = renderer;
		this.renderer.AddRenderable(this);
	}

	public void Render(SKCanvas canvas)
	{
		using var paint = new SKPaint { Color = SKColors.White, TextSize = PlayerHealth.Height };
		canvas.DrawText($"Score: {Score}", 0.0f, 0.0f, paint);
	}

	public void Dispose() =>
		renderer.RemoveRenderable(this);
}
