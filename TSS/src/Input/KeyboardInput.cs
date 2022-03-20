using Zarya;
using Zarya.Input;

#nullable disable

namespace TSS;

record KeyboardInput(IInputManager inputManager) : InputBase(inputManager), IInput
{
	[KeyboardAxisInput(Key.S, Key.W)]
	public InputAxis Vertical { get; }

	[KeyboardAxisInput(Key.A, Key.D)]
	public InputAxis Horizontal { get; }

	[KeyboardAxisInput(Key.Down, Key.Up)]
	public InputAxis LookVertical { get; }

	[KeyboardAxisInput(Key.Left, Key.Right)]
	public InputAxis LookHorizontal { get; }

	[KeyboardInput(Key.Space)]
	public InputButton Fire { get; }
}
