using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

namespace Zarya.Silk.NET;

/// <summary>
/// <see cref="SilkWindow"/> is a game manager and input manager backed by Silk.NET.
/// </summary>
public partial class SilkWindow : IGameManager, IGLContextSource, IDisposable
{
	private const float MillisecondsPerSecond = 1000.0f;

	private readonly IServiceScopeFactory serviceScopeFactory;

	private IWindow? window = null;
	/// <summary>
	/// The <see cref="IGLContext"/> used by this <see cref="SilkWindow"/>.
	/// </summary>
	public IGLContext? GLContext => window?.GLContext;
	/// <summary>
	/// The width of the render area.
	/// </summary>
	public int? Width => window?.Size.X;
	/// <summary>
	/// The height of the render area.
	/// </summary>
	public int? Height => window?.Size.Y;

	/// <summary>
	/// Silk.NET window options that can be used to customize the window before it is initialized.
	/// </summary>
	public WindowOptions WindowOptions { get; set; } = WindowOptions.Default;

	/// <inheritdoc/>
	public float Time => (float?)window?.Time ?? 0.0f;

	private long targetDeltaTimeMillis = 16;
	/// <summary>
	/// The target delta time in seconds.
	/// </summary>
	public float TargetDeltaTime
	{
		get => targetDeltaTimeMillis / MillisecondsPerSecond;
		set => targetDeltaTimeMillis = (long)(value * MillisecondsPerSecond);
	}

	private readonly IDictionary<object, IServiceScope> objects = new Dictionary<object, IServiceScope>(ReferenceEqualityComparer.Instance);

	private event Action? initialize;
	/// <inheritdoc/>
	public event Action? Initialize
	{
		add
		{
			if (window?.IsInitialized == true)
			{
				value?.Invoke();
			}
			else
			{
				initialize += value;
			}
		}
		remove => initialize -= value;
	}

	/// <inheritdoc/>
	public event Action<float>? Update;

	/// <summary>
	/// Called once pre render loop iteration.
	/// </summary>
	public event Action? Render;

	public SilkWindow(IServiceScopeFactory serviceScopeFactory)
	{
		this.serviceScopeFactory = serviceScopeFactory;
	}

	void IGameManager.Run()
	{
		window = Window.Create(WindowOptions with
		{
			FramesPerSecond = 1.0d / TargetDeltaTime,
		});

		window.Load += OnLoad;
		window.Update += OnUpdate;
		window.Render += OnRender;
		window.Run();
	}

	private void OnLoad()
	{
		InitializeInput();
		initialize?.Invoke();
	}

	private void OnUpdate(double deltaTime)
	{
		Update?.Invoke((float)deltaTime);
		UpdateInput();
	}

	private void OnRender(double deltaTime) =>
		Render?.Invoke();

	/// <inheritdoc/>
	public T? Create<T>(params object[] parameters) where T : class
	{
		var scope = serviceScopeFactory.CreateScope();
		var obj = ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider, parameters);
		if (obj is T t)
		{
			objects.Add(t, scope);
			return t;
		}
		return null;
	}

	/// <inheritdoc/>
	public bool Destroy<T>(T obj) where T : class
	{
		if (objects.TryGetValue(obj, out var scope))
		{
			objects.Remove(obj);
			if (obj is IDisposable disposable)
			{
				disposable.Dispose();
			}
			scope.Dispose();

			return true;
		}
		else
		{
			return false;
		}
	}

	/// <inheritdoc/>
	public void Close()
	{
		window?.Close();

		foreach (var scope in objects.Values)
		{
			scope.Dispose();
		}
		objects.Clear();
	}

	public void Dispose()
	{
		DisposeInput();
		Close();
		window?.Dispose();
	}
}
