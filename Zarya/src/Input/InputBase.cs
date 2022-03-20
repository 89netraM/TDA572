using System;
using System.Linq;
using System.Reflection;

namespace Zarya.Input;

/// <summary>
/// Base class for all input mappings. Extend this class to create custom input mappings.
/// </summary>
/// <example>
/// <code>
/// record MyInput(IInputManager inputManager) : InputBase(inputManager)
/// {
/// 	[KeyboardInput(Key.Space, InputState.Down)]
/// 	[GamepadButtonInput(Button.South, InputState.Down)]
/// 	public InputButton MyButton { get; }
///
/// 	[KeyboardAxisInput(Key.Left, Key.Right)]
/// 	[GamepadAxisInput(GamepadAxis.LeftStickX)]
/// 	public InputAxis MyAxis { get; }
/// }
/// </code>
/// </example>
public abstract record InputBase
{
	/// <summary>
	/// Indicates that input from any gamepad should be accepted.
	/// </summary>
	public const int AnyGamepad = -1;

	protected IInputManager InputManager { get; }

	protected int Player { get; }

	public InputBase(IInputManager inputManager, int player = AnyGamepad)
	{
		InputManager = inputManager;
		Player = player;

		foreach (var prop in GetType().GetProperties())
		{
			var attrs = (InputAttribute[])prop.GetCustomAttributes(typeof(InputAttribute), true);
			if (attrs.Length > 0)
			{
				var method = attrs.Select<InputAttribute, InputButton>(attr => () => attr.GetValue(InputManager, Player))
					.Aggregate((a, b) => () => a() || b());

				SetMethod(this, prop, method);
			}

			var axisAttrs = (AxisInputAttribute[])prop.GetCustomAttributes(typeof(AxisInputAttribute), true);
			if (axisAttrs.Length > 0)
			{
				var method = axisAttrs.Select<AxisInputAttribute, InputAxis>(attr => () => attr.GetValue(InputManager, Player))
					.Aggregate((a, b) => () => a() + b());

				SetMethod(this, prop, new InputAxis(() => Math.Clamp(method(), -1.0f, 1.0f)));
			}
		}

		static void SetMethod(InputBase inputBase, PropertyInfo prop, object value)
		{
			var fieldName = $"<{prop.Name}>k__BackingField";
			var field = prop.DeclaringType?.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (field is null)
			{
				throw new Exception($"Could not find backing field ({fieldName}) for {prop.Name}");
			}
			field.SetValue(inputBase, value);
		}
	}
}
