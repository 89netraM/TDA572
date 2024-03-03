using System;

namespace Zarya.Input;

#pragma warning disable CS9113

/// <summary>
/// Attribute for mapping a mouse button to an input action.
/// </summary>
/// <param name="button">The mouse button used.</param>
/// <param name="state">The input state to check for. Default is <see cref="InputState.Pressed"/>.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MouseInputAttribute(MouseButton button, InputState state = InputState.Pressed) : Attribute;
