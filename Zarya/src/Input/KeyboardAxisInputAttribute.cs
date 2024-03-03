using System;

namespace Zarya.Input;

#pragma warning disable CS9113

/// <summary>
/// Attribute for mapping two keyboard keys to an input action as an axis.
/// </summary>
/// <param name="negative">The keyboard key used for the negative value.</param>
/// <param name="positive">The keyboard key used for the positive value.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class KeyboardAxisInputAttribute(Key negative, Key positive) : Attribute;
