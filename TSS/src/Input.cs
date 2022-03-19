using Zarya;
using Zarya.Input;

#nullable disable

namespace TSS;

record Input(IInputManager inputManager) : InputBase(inputManager)
{
	[KeyboardAxisInput(Key.S, Key.W)]
	[GamepadAxisInput(GamepadAxis.LeftStickY)]
	public InputAxis Vertical { get; }

	[KeyboardAxisInput(Key.A, Key.D)]
	[GamepadAxisInput(GamepadAxis.LeftStickX)]
	public InputAxis Horizontal { get; }

	[KeyboardAxisInput(Key.Down, Key.Up)]
	[GamepadAxisInput(GamepadAxis.RightStickY)]
	public InputAxis LookVertical { get; }

	[KeyboardAxisInput(Key.Left, Key.Right)]
	[GamepadAxisInput(GamepadAxis.RightStickX)]
	public InputAxis LookHorizontal { get; }

	public bool Fire() => InputManager.IsKeyPressed(Key.Space) || InputManager.GetGamepadAxis(GamepadAxis.Trigger) > 0.9f;

	[KeyboardInput(Key.Escape, InputState.Down)]
	[GamepadButtonInput(Button.Start, InputState.Down)]
	public InputButton Pause { get; }
}
