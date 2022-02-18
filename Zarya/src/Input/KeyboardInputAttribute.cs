namespace Zarya.Input;

/// <summary>
/// Attribute for mapping a keyboard key to an input action.
/// </summary>
public class KeyboardInputAttribute : InputAttribute
{
	/// <summary>
	/// The keyboard key used.
	/// </summary>
	public Key Key { get; }
	/// <summary>
	/// The input state to check for.
	/// </summary>
	public InputState State { get; }

	/// <param name="key">The keyboard key used.</param>
	/// <param name="state">The input state to check for. Default is <see cref="InputState.Pressed"/>.</param>
	public KeyboardInputAttribute(Key key, InputState state = InputState.Pressed) =>
		(Key, State) = (key, state);

	public override bool GetValue(IInputManager inputManager) =>
		inputManager.IsKey(Key, State);
}
