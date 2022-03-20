using Zarya;
using Zarya.Input;

#nullable disable

namespace TSS;

record GamepadInput(IInputManager inputManager, int player) : InputBase(inputManager, player), IInput
{
	[GamepadAxisInput(GamepadAxis.LeftStickY)]
	public InputAxis Vertical { get; }

	[GamepadAxisInput(GamepadAxis.LeftStickX)]
	public InputAxis Horizontal { get; }

	[GamepadAxisInput(GamepadAxis.RightStickY)]
	public InputAxis LookVertical { get; }

	[GamepadAxisInput(GamepadAxis.RightStickX)]
	public InputAxis LookHorizontal { get; }

	public InputButton Fire => () => (Player == InputBase.AnyGamepad ?
		InputManager.GetGamepadAxis(GamepadAxis.Trigger) :
		InputManager.GetGamepadAxis(GamepadAxis.Trigger, Player)) > 0.9f;
}
