namespace Zarya.Input;

/// <summary>
/// Attribute for mapping a gamepad axis to an input action.
/// </summary>
public class GamepadAxisInputAttribute : AxisInputAttribute
{
	/// <summary>
	/// The gamepad axis used.
	/// </summary>
	public GamepadAxis Axis { get; }

	/// <param name="axis">The gamepad axis used.</param>
	public GamepadAxisInputAttribute(GamepadAxis axis) =>
		Axis = axis;

	public override float GetValue(IInputManager inputManager) =>
		inputManager.GetGamepadAxis(Axis);
}
