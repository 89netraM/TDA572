using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ComputeSharp;
using Silk.NET.OpenGL;
using Zarya.Silk.NET;

namespace Zarya.RayTracer;

public partial class RayTracerRenderer : IDisposable
{
	private readonly SilkWindow silkWindow;
	private RenderingContext? renderingContext = null;

	private IList<IRayTraceRenderable> renderables = new List<IRayTraceRenderable>();

	public float FOV { get; set; } = 90.0f;

	public RayTracerRenderer(SilkWindow silkWindow)
	{
		this.silkWindow = silkWindow;
		this.silkWindow.Initialize += OnInitialize;
		this.silkWindow.Render += OnRender;
	}

	private void OnInitialize()
	{
		renderingContext = new RenderingContext(GL.GetApi(silkWindow.GLContext), silkWindow.Width ?? 1, silkWindow.Height ?? 1);
	}

	private void OnRender()
	{
		if (renderingContext is RenderingContext rc)
		{
			rc.FOV = FOV;
			rc.Width = silkWindow.Width ?? 1;
			rc.Height = silkWindow.Height ?? 1;

			var rayTracerObjects = new RayTracerObject[renderables.Count];
			for (int i = 0; i < renderables.Count; i++)
			{
				rayTracerObjects[i] = renderables[i].TraceableObject;
			}
			rc.Render(silkWindow.Time, rayTracerObjects);
		}
	}

	/// <summary>
	/// Adds a renderable to the renderer, to be rendered on the next render.
	/// </summary>
	public void AddRenderable(IRayTraceRenderable renderable) =>
		renderables.Add(renderable);

	/// <summary>
	/// Removes a renderable from the renderer.
	/// </summary>
	public void RemoveRenderable(IRayTraceRenderable renderable) =>
		renderables.Remove(renderable);

	public void Dispose()
	{
		silkWindow.Initialize -= OnInitialize;
		silkWindow.Render -= OnRender;
		renderingContext?.Dispose();
	}

	class RenderingContext : IDisposable
	{
		private readonly GL gl;

		public float FOV { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		private ReadWriteTexture2D<Rgba32, float4>? texture;
		private Rgba32[]? pixels;
		private uint fbo;
		private uint glTexture;

		public RenderingContext(GL gl, int width, int height)
		{
			this.gl = gl;
			Width = width;
			Height = height;

			fbo = gl.GenFramebuffer();
			glTexture = gl.GenTexture();
		}

		public void Render(float time, RayTracerObject[] rayTracerObjects)
		{
			EnsureTextureSize();

			var gd = GraphicsDevice.GetDefault();
			var rayTracerObjectBuffer = gd.AllocateReadOnlyBuffer<RayTracerObject>(rayTracerObjects);
			gd.ForEach(texture, new Shader(time, FOV, (float)Width / Height, rayTracerObjectBuffer));

			texture.CopyTo(pixels);
			gl.BindTexture(TextureTarget.Texture2D, glTexture);
			unsafe
			{
				fixed (void* p = pixels)
				{
					gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (uint)Width, (uint)Height, PixelFormat.Rgba, PixelType.UnsignedByte, p);
				}
			}

			gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, fbo);
			gl.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, glTexture, 0);
			gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
			gl.BlitFramebuffer(0, 0, Width, Height, 0, 0, Width, Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
		}

		[MemberNotNull(nameof(texture), nameof(pixels))]
		private void EnsureTextureSize()
		{
			if (pixels?.Length != Width * Height)
			{
				pixels = new Rgba32[Width * Height];
			}

			if (texture?.Width != Width || texture?.Height != Height)
			{
				texture?.Dispose();
				texture = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<Rgba32, float4>(Width, Height);

				gl.BindTexture(TextureTarget.Texture2D, glTexture);
				unsafe
				{
					gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)Width, (uint)Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
				}
			}
		}

		public void Dispose()
		{
			gl.DeleteFramebuffer(fbo);
			gl.DeleteTexture(glTexture);
			texture?.Dispose();
			gl.Dispose();
		}
	}
}
