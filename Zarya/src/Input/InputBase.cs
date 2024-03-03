namespace Zarya.Input;

/// <summary>
/// Base class for all input mappings. Extend this class to create custom input mappings.
/// </summary>
/// <example>
/// <code>
/// partial class MyInput(IInputManager inputManager) : InputBase(inputManager)
/// {
/// 	[KeyboardInput(Key.Space, InputState.Down)]
/// 	[GamepadButtonInput(Button.South, InputState.Down)]
/// 	public partial bool MyButton();
///
/// 	[KeyboardAxisInput(Key.Left, Key.Right)]
/// 	[GamepadAxisInput(GamepadAxis.LeftStickX)]
/// 	public partial float MyAxis();
/// }
/// </code>
/// </example>
public abstract class InputBase(IInputManager inputManager, int player = InputBase.AnyGamepad)
{
	/// <summary>
	/// Indicates that input from any gamepad should be accepted.
	/// </summary>
	public const int AnyGamepad = -1;

	protected IInputManager InputManager { get; } = inputManager;

	protected int Player { get; } = player;
}
