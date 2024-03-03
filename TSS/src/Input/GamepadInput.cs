using Zarya;
using Zarya.Input;

namespace TSS;

partial class GamepadInput(IInputManager inputManager, int player) : InputBase(inputManager, player), IInput
{
	[GamepadAxisInput(GamepadAxis.LeftStickY)]
	public partial float Vertical();

	[GamepadAxisInput(GamepadAxis.LeftStickX)]
	public partial float Horizontal();

	[GamepadAxisInput(GamepadAxis.RightStickY)]
	public partial float LookVertical();

	[GamepadAxisInput(GamepadAxis.RightStickX)]
	public partial float LookHorizontal();

	public bool Fire() => (Player == InputBase.AnyGamepad ?
			InputManager.GetGamepadAxis(GamepadAxis.Trigger) :
			InputManager.GetGamepadAxis(GamepadAxis.Trigger, Player)) > 0.9f;
}
