using Zarya;
using Zarya.Input;

#nullable disable

namespace GameTest;

record Input(IInputManager inputManager) : InputBase(inputManager)
{
	[KeyboardAxisInput(Key.Down, Key.Up)]
	[KeyboardAxisInput(Key.S, Key.W)]
	[GamepadAxisInput(GamepadAxis.LeftStickY)]
	public InputAxis Vertical { get; }

	[KeyboardAxisInput(Key.Left, Key.Right)]
	[KeyboardAxisInput(Key.A, Key.D)]
	[GamepadAxisInput(GamepadAxis.Trigger)]
	public InputAxis Horizontal { get; }

	[KeyboardInput(Key.Space, InputState.Down)]
	[GamepadButtonInput(Button.South, InputState.Down)]
	public InputButton Fire { get; }

	[KeyboardInput(Key.R, InputState.Down)]
	[GamepadButtonInput(Button.North, InputState.Down)]
	public InputButton Reset { get; }
}
