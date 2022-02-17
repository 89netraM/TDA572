using System;
using System.Collections.Generic;
using SkiaSharp;
using Zarya.Silk.NET;

namespace Zarya.SkiaSharp;

/// <summary>
/// A 2D renderer backed by SkiaSharp.
/// </summary>
public class SkiaSharpRenderer : IDisposable
{
	private readonly SilkWindow silkWindow;
	private RenderingContext? renderingContext = null;

	private readonly List<ISkiaSharpRenderable> renderables = new List<ISkiaSharpRenderable>();

	public SkiaSharpRenderer(SilkWindow silkWindow)
	{
		this.silkWindow = silkWindow;
		this.silkWindow.Initialize += OnInitialize;
		this.silkWindow.Render += OnRender;
	}

	private void OnInitialize()
	{
		renderingContext = new RenderingContext(
			silkWindow.Width ?? 0,
			silkWindow.Height ?? 0,
			name =>
				silkWindow.GLContext?.TryGetProcAddress(name, out var addr) ?? false ?
					addr :
					(IntPtr)0
			);
	}

	private void OnRender()
	{
		if (renderingContext is RenderingContext rc)
		{
			if (rc.Width != silkWindow.Width || rc.Height != silkWindow.Height)
			{
				rc.Resize(silkWindow.Width ?? 0, silkWindow.Height ?? 0);
			}

			rc.Render(renderables);
		}
	}

	/// <summary>
	/// Adds a renderable to the renderer, to be rendered on the next render.
	/// </summary>
	public void AddRenderable(ISkiaSharpRenderable renderable) =>
		renderables.Add(renderable);

	/// <summary>
	/// Removes a renderable from the renderer.
	/// </summary>
	public void RemoveRenderable(ISkiaSharpRenderable renderable) =>
		renderables.Remove(renderable);

	public void Dispose()
	{
		silkWindow.Initialize -= OnInitialize;
		silkWindow.Render -= OnRender;
		renderingContext?.Dispose();
	}

	class RenderingContext : IDisposable
	{
		private static (GRBackendRenderTarget, SKSurface, SKCanvas) CreateRenderTarget(GRContext grContext, int width, int height)
		{
			var renderTarget = new GRBackendRenderTarget(width, height, 0, 8, new GRGlFramebufferInfo(0, 0x8058));
			var surface = SKSurface.Create(grContext, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
			return (renderTarget, surface, surface.Canvas);
		}

		private readonly GRGlInterface grGlInterface;
		private readonly GRContext grContext;
		private GRBackendRenderTarget renderTarget;
		private SKSurface surface;
		private SKCanvas canvas;

		public int Width => renderTarget.Width;
		public int Height => renderTarget.Height;

		public RenderingContext(int width, int height, GRGlGetProcedureAddressDelegate getProcAddress)
		{
			grGlInterface = GRGlInterface.Create(getProcAddress);
			grContext = GRContext.CreateGl(grGlInterface);
			(renderTarget, surface, canvas) = CreateRenderTarget(grContext, width, height);
		}

		public void Render(IReadOnlyList<ISkiaSharpRenderable> renderables)
		{
			grContext.ResetContext();
			canvas.Clear(SKColors.Black);

			foreach (var renderable in renderables)
			{
				canvas.Save();
				canvas.Translate(renderable.Transform.Position.X, renderable.Transform.Position.Y);
				canvas.RotateRadians(renderable.Transform.Rotation);
				renderable.Render(canvas);
				canvas.Restore();
			}

			canvas.Flush();
		}

		public void Resize(int width, int height)
		{
			canvas.Dispose();
			surface.Dispose();
			renderTarget.Dispose();
			(renderTarget, surface, canvas) = CreateRenderTarget(grContext, width, height);
		}

		public void Dispose()
		{
			canvas.Dispose();
			surface.Dispose();
			renderTarget.Dispose();
			grContext.Dispose();
			grGlInterface.Dispose();
		}
	}
}
