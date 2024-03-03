using Zarya;
using Zarya.Input;

namespace GameTest;

partial class Input(IInputManager inputManager) : InputBase(inputManager)
{
	[KeyboardAxisInput(Key.Down, Key.Up)]
	[KeyboardAxisInput(Key.S, Key.W)]
	[GamepadAxisInput(GamepadAxis.LeftStickY)]
	public partial float Vertical();

	[KeyboardAxisInput(Key.Left, Key.Right)]
	[KeyboardAxisInput(Key.A, Key.D)]
	[GamepadAxisInput(GamepadAxis.Trigger)]
	public partial float Horizontal();

	[KeyboardInput(Key.Space, InputState.Down)]
	[GamepadButtonInput(Button.South, InputState.Down)]
	public partial bool Fire();

	[KeyboardInput(Key.R, InputState.Down)]
	[GamepadButtonInput(Button.North, InputState.Down)]
	public partial bool Reset();
}
