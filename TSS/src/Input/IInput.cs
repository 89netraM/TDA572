using Zarya.Input;

namespace TSS;

interface IInput
{
	InputAxis Vertical { get; }
	InputAxis Horizontal { get; }
	InputAxis LookVertical { get; }
	InputAxis LookHorizontal { get; }
	InputButton Fire { get; }
}
