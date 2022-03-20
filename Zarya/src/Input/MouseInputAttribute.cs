namespace Zarya.Input;

/// <summary>
/// Attribute for mapping a mouse button to an input action.
/// </summary>
public class MouseInputAttribute : InputAttribute
{
	/// <summary>
	/// The mouse button used.
	/// </summary>
	public MouseButton Button { get; }
	/// <summary>
	/// The input state to check for.
	/// </summary>
	public InputState State { get; }

	/// <param name="button">The mouse button used.</param>
	/// <param name="state">The input state to check for. Default is <see cref="InputState.Pressed"/>.</param>
	public MouseInputAttribute(MouseButton button, InputState state = InputState.Pressed) =>
		(Button, State) = (button, state);

	public override bool GetValue(IInputManager inputManager, int _) =>
		inputManager.IsMouseButton(Button, State);
}
