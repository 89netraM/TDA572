namespace Zarya.Input;

/// <summary>
/// Attribute for mapping a gamepad button to an input action.
/// </summary>
public class GamepadButtonInputAttribute : InputAttribute
{
	/// <summary>
	/// The gamepad button used.
	/// </summary>
	public Button Button { get; }
	/// <summary>
	/// The input state to check for.
	/// </summary>
	public InputState State { get; }

	/// <param name="button">The gamepad button used.</param>
	/// <param name="state">The input state to check for. Default is <see cref="InputState.Pressed"/>.</param>
	public GamepadButtonInputAttribute(Button button, InputState state = InputState.Pressed) =>
		(Button, State) = (button, state);

	public override bool GetValue(IInputManager inputManager, int player) =>
		player == InputBase.AnyGamepad ?
			inputManager.IsButton(Button, State) :
			inputManager.IsButton(Button, State, player);
}
