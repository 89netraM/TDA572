using System;

namespace Zarya.Input;

/// <summary>
/// Base attribute for mapping an axis to an input action.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class AxisInputAttribute : Attribute
{
	/// <summary>
	/// Gets the value of the axis. In the range of -1 to 1.
	/// </summary>
	public abstract float GetValue(IInputManager inputManager);
}
