using System;
using System.Numerics;
using SkiaSharp;
using Zarya;
using Zarya.Silk.NET;
using Zarya.SkiaSharp;

namespace TSS;

class PlayerHealth : ISkiaSharpRenderable, IDisposable
{
	public const float Margin = 10.0f;
	public const float Height = 30.0f;
	public const float Width = 120.0f;
	public const float Padding = 2.5f;

	private readonly Transform2D transform;
	Transform2D ISkiaSharpRenderable.Transform => transform;
	private readonly SkiaSharpRenderer renderer;
	private readonly SilkWindow window;

	private PlayerHealthPosition position;
	public PlayerHealthPosition Position
	{
		get => position;
		set
		{
			position = value;
			transform.Position = value switch
			{
				PlayerHealthPosition.TopLeft => new Vector2(Margin + Width / 2.0f, Margin + Height / 2.0f),
				PlayerHealthPosition.TopRight => new Vector2((window.Width ?? 800.0f) - (Margin + Width / 2.0f), Margin + Height / 2.0f),
				PlayerHealthPosition.BottomLeft => new Vector2(Margin + Width / 2.0f, (window.Height ?? 800.0f) - (Margin + Height / 2.0f)),
				PlayerHealthPosition.BottomRight => new Vector2((window.Width ?? 800.0f) - (Margin + Width / 2.0f), (window.Height ?? 800.0f) - (Margin + Height / 2.0f)),
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}

	public SKColor Color { get; set; } = SKColors.Black;
	public float Health { get; set; } = 1.0f;

	public PlayerHealth(
		Transform2D transform,
		SkiaSharpRenderer renderer,
		SilkWindow window
	)
	{
		this.transform = transform;

		this.renderer = renderer;
		this.renderer.AddRenderable(this);

		this.window = window;

		Position = PlayerHealthPosition.TopLeft;
	}

	void ISkiaSharpRenderable.Render(SKCanvas canvas)
	{
		using (var paint = new SKPaint { Color = SKColors.LightGray })
		{
			canvas.DrawRect(-Width / 2.0f, -Height / 2.0f, Width, Height, paint);
		}
		using (var paint = new SKPaint { Color = Color })
		{
			canvas.DrawRect(-Width / 2.0f + Padding, -Height / 2.0f + Padding, (Width - 2.0f * Padding) * Health, Height - 2.0f * Padding, paint);
		}
	}

	public void Dispose() =>
		renderer.RemoveRenderable(this);
}

enum PlayerHealthPosition
{
	TopLeft,
	TopRight,
	BottomLeft,
	BottomRight
}
