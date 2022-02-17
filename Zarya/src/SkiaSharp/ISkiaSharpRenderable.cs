using SkiaSharp;

namespace Zarya.SkiaSharp;

/// <summary>
/// An interface for objects that can be rendered by SkiaSharp.
/// </summary>
public interface ISkiaSharpRenderable
{
	/// <summary>
	/// Renders this object with the transforms position and rotation.
	/// </summary>
	Transform2D Transform { get; }

	/// <summary>
	/// Provided with a <see cref="SKCanvas"/>, renders this object to the center of the canvas.
	/// </summary>
	void Render(SKCanvas canvas);
}
