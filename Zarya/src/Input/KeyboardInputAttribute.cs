using System;

namespace Zarya.Input;

#pragma warning disable CS9113

/// <summary>
/// Attribute for mapping a keyboard key to an input action.
/// </summary>
/// <param name="key">The keyboard key used.</param>
/// <param name="state">The input state to check for. Default is <see cref="InputState.Pressed"/>.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class KeyboardInputAttribute(Key key, InputState state = InputState.Pressed) : Attribute;
