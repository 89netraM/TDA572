using System;

namespace Zarya.Input;

/// <summary>
/// Base attribute for mapping a button/key to an input action.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class InputAttribute : Attribute
{
	public abstract bool GetValue(IInputManager inputManager, int player);
}
