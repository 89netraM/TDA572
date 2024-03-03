using System;

namespace Zarya.Input;

#pragma warning disable CS9113

/// <summary>
/// Attribute for mapping a gamepad axis to an input action.
/// </summary>
/// <param name="axis">The gamepad axis used.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class GamepadAxisInputAttribute(GamepadAxis axis) : Attribute;
