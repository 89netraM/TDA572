using Zarya;
using Zarya.Input;

namespace TSS;

class AnyInput(IInputManager inputManager) : IInput
{
	private readonly KeyboardInput keyboardInput = new(inputManager);
	private readonly GamepadInput gamepadInput = new(inputManager, InputBase.AnyGamepad);

	public float Vertical() => keyboardInput.Vertical() != 0.0f ? keyboardInput.Vertical() : gamepadInput.Vertical();

	public float Horizontal() => keyboardInput.Horizontal() != 0.0f ? keyboardInput.Horizontal() : gamepadInput.Horizontal();

	public float LookVertical() => keyboardInput.LookVertical() != 0.0f ? keyboardInput.LookVertical() : gamepadInput.LookVertical();

	public float LookHorizontal() => keyboardInput.LookHorizontal() != 0.0f ? keyboardInput.LookHorizontal() : gamepadInput.LookHorizontal();

	public bool Fire() => keyboardInput.Fire() || gamepadInput.Fire();
}
