using System;
using SkiaSharp;

namespace Zarya.SkiaSharp;

/// <summary>
/// A sprite that can be drawn as a <see cref="ISkiaSharpRenderable"/>.
/// </summary>
public class SkiaSharpSprite : ISkiaSharpRenderable, IDisposable
{
	private static readonly SKPoint center = new SKPoint(0.0f, 0.0f);

	private readonly Transform2D transform;
	Transform2D ISkiaSharpRenderable.Transform => transform;

	private readonly SkiaSharpRenderer renderer;

	/// <summary>
	/// The path to the sprite's texture. Set to null to disable rendering.
	/// </summary>
	public string? Sprite { get; set; } = null;
	private string? activeSprite = null;
	private SKBitmap? bitmap = null;

	public SkiaSharpSprite(SkiaSharpRenderer renderer, Transform2D transform)
	{
		this.transform = transform;

		this.renderer = renderer;
		this.renderer.AddRenderable(this);
	}

	void ISkiaSharpRenderable.Render(SKCanvas canvas)
	{
		if (bitmap is null || activeSprite != Sprite)
		{
			if (Sprite is null)
			{
				return;
			}

			bitmap?.Dispose();
			bitmap = SKBitmap.Decode(Sprite);
			activeSprite = Sprite;
		}
		canvas.DrawBitmap(bitmap, center - new SKPoint(bitmap.Width / 2.0f, bitmap.Height / 2.0f));
	}

	public void Dispose()
	{
		renderer.RemoveRenderable(this);
		bitmap?.Dispose();
	}
}
