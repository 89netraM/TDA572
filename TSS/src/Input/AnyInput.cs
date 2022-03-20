using Zarya;
using Zarya.Input;

namespace TSS;

class AnyInput : IInput
{
	private readonly KeyboardInput keyboardInput;
	private readonly GamepadInput gamepadInput;

	public AnyInput(IInputManager inputManager)
	{
		keyboardInput = new KeyboardInput(inputManager);
		gamepadInput = new GamepadInput(inputManager, InputBase.AnyGamepad);
	}

	public InputAxis Vertical => () => keyboardInput.Vertical() != 0.0f ? keyboardInput.Vertical() : gamepadInput.Vertical();

	public InputAxis Horizontal => () => keyboardInput.Horizontal() != 0.0f ? keyboardInput.Horizontal() : gamepadInput.Horizontal();

	public InputAxis LookVertical => () => keyboardInput.LookVertical() != 0.0f ? keyboardInput.LookVertical() : gamepadInput.LookVertical();

	public InputAxis LookHorizontal => () => keyboardInput.LookHorizontal() != 0.0f ? keyboardInput.LookHorizontal() : gamepadInput.LookHorizontal();

	public InputButton Fire => () => keyboardInput.Fire() || gamepadInput.Fire();
}
