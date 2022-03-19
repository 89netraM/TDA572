using System;
using System.Numerics;
using SkiaSharp;
using TSS;
using Zarya;
using Zarya.SkiaSharp;

class ButtonTarget : ISkiaSharpRenderable, IDisposable
{
	private const float Size = 64.0f;

	private readonly Transform2D transform;
	Transform2D ISkiaSharpRenderable.Transform => transform;
	private readonly PhysicsBody2D physicsBody;
	private readonly IMessageReceiver messageReceiver;

	private readonly SkiaSharpRenderer renderer;
	private readonly string text;
	private readonly SKColor color;

	public event Action? Activate;

	public ButtonTarget(
		Transform2D transform,
		PhysicsBody2D physicsBody,
		IMessageReceiver messageReceiver,
		TagManager tagManager,
		SkiaSharpRenderer renderer,
		Vector2 position,
		string text,
		SKColor color
	)
	{
		this.transform = transform;
		this.transform.Position = position;

		this.physicsBody = physicsBody;
		this.physicsBody.IsKinematic = true;
		this.physicsBody.AddCollider<CircleCollider>(Vector2.Zero, Size);

		this.messageReceiver = messageReceiver;
		this.messageReceiver.Message += OnMessage;

		tagManager.AddTag(Tags.Target);

		this.renderer = renderer;
		this.renderer.AddRenderable(this);
		this.text = text;
		this.color = color;
	}

	private void OnMessage(object message)
	{
		if (message is BulletHit)
		{
			Activate?.Invoke();
		}
	}

	void ISkiaSharpRenderable.Render(SKCanvas canvas)
	{
		using (var paint = new SKPaint { Color = color })
		{
			canvas.DrawCircle(0.0f, 0.0f, Size, paint);
		}
		using (var paint = new SKPaint { Color = SKColors.White, TextSize = 32.0f })
		{
			paint.GetFontMetrics(out var fontMetrics);
			canvas.DrawText(text, -paint.MeasureText(text) / 2.0f, fontMetrics.CapHeight / 2.0f, paint);
		}
	}

	public void Dispose()
	{
		messageReceiver.Message -= OnMessage;
		renderer.RemoveRenderable(this);
	}
}
