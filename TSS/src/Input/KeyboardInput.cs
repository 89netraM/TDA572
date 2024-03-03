using Zarya;
using Zarya.Input;

namespace TSS;

partial class KeyboardInput(IInputManager inputManager) : InputBase(inputManager), IInput
{
	[KeyboardAxisInput(Key.S, Key.W)]
	public partial float Vertical();

	[KeyboardAxisInput(Key.A, Key.D)]
	public partial float Horizontal();

	[KeyboardAxisInput(Key.Down, Key.Up)]
	public partial float LookVertical();

	[KeyboardAxisInput(Key.Left, Key.Right)]
	public partial float LookHorizontal();

	[KeyboardInput(Key.Space)]
	public partial bool Fire();
}
