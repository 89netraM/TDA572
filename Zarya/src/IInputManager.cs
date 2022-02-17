using System.Numerics;

namespace Zarya;

/// <summary>
/// A interface for reading the current state of input devices.
/// </summary>
public interface IInputManager
{
	/// <summary>
	/// Were <paramref name="key"/> <paramref name="state"/> this frame?
	/// </summary>
	bool IsKey(Key key, InputState state) => state switch
	{
		InputState.Down => IsKeyDown(key),
		InputState.Pressed => IsKeyPressed(key),
		InputState.Up => IsKeyUp(key),
		_ => false
	};
	/// <summary>
	/// Were <paramref name="key"/> pressed this frame?
	/// </summary>
	bool IsKeyDown(Key key);
	/// <summary>
	/// Is <paramref name="key"/> held down this frame?
	/// </summary>
	bool IsKeyPressed(Key key);
	/// <summary>
	/// Were <paramref name="key"/> released this frame?
	/// </summary>
	bool IsKeyUp(Key key);

	/// <summary>
	/// The current position of the mouse cursor.
	/// </summary>
	Vector2? GetMousePosition();
	/// <summary>
	/// Were <paramref name="button"/> <paramref name="state"/> this frame?
	/// </summary>
	bool IsMouseButton(MouseButton button, InputState state) => state switch
	{
		InputState.Down => IsMouseButtonDown(button),
		InputState.Pressed => IsMouseButtonPressed(button),
		InputState.Up => IsMouseButtonUp(button),
		_ => false
	};
	/// <summary>
	/// Were <paramref name="button"/> pressed this frame?
	/// </summary>
	bool IsMouseButtonDown(MouseButton button);
	/// <summary>
	/// Is <paramref name="button"/> held down this frame?
	/// </summary>
	bool IsMouseButtonPressed(MouseButton button);
	/// <summary>
	/// Were <paramref name="button"/> released this frame?
	/// </summary>
	bool IsMouseButtonUp(MouseButton button);

	/// <summary>
	/// Were <paramref name="button"/> <paramref name="state"/> this frame?
	/// </summary>
	bool IsButton(Button button, InputState state) => state switch
	{
		InputState.Down => IsButtonDown(button),
		InputState.Pressed => IsButtonPressed(button),
		InputState.Up => IsButtonUp(button),
		_ => false
	};
	/// <summary>
	/// Were <paramref name="button"/> <paramref name="state"/> this frame for the given <paramref name="player"/>?
	/// </summary>
	bool IsButton(Button button, InputState state, int player) => state switch
	{
		InputState.Down => IsButtonDown(button, player),
		InputState.Pressed => IsButtonPressed(button, player),
		InputState.Up => IsButtonUp(button, player),
		_ => false
	};
	/// <summary>
	/// Were <paramref name="button"/> pressed this frame?
	/// </summary>
	bool IsButtonDown(Button button);
	/// <summary>
	/// Were <paramref name="button"/> pressed this frame by <paramref name="player"/>?
	/// </summary>
	bool IsButtonDown(Button button, int player);
	/// <summary>
	/// Is <paramref name="button"/> held down this frame?
	/// </summary>
	bool IsButtonPressed(Button button);
	/// <summary>
	/// Is <paramref name="button"/> held down this frame by <paramref name="player"/>?
	/// </summary>
	bool IsButtonPressed(Button button, int player);
	/// <summary>
	/// Were <paramref name="button"/> released this frame?
	/// </summary>
	bool IsButtonUp(Button button);
	/// <summary>
	/// Were <paramref name="button"/> released this frame by <paramref name="player"/>?
	/// </summary>
	bool IsButtonUp(Button button, int player);
}

public enum MouseButton
{
	Left,
	Right,
	Middle,
	Button4,
	Button5,
	Button6,
	Button7,
	Button8,
	Button9,
	Button10,
	Button11,
	Button12,
}

public enum Key
{
	Space = 32,
	Apostrophe = 39,
	Comma = 44,
	Minus = 45,
	Period = 46,
	Slash = 47,
	Number0 = 48,
	Number1 = 49,
	Number2 = 50,
	Number3 = 51,
	Number4 = 52,
	Number5 = 53,
	Number6 = 54,
	Number7 = 55,
	Number8 = 56,
	Number9 = 57,
	Semicolon = 59,
	Equal = 61,
	A = 65,
	B = 66,
	C = 67,
	D = 68,
	E = 69,
	F = 70,
	G = 71,
	H = 72,
	I = 73,
	J = 74,
	K = 75,
	L = 76,
	M = 77,
	N = 78,
	O = 79,
	P = 80,
	Q = 81,
	R = 82,
	S = 83,
	T = 84,
	U = 85,
	V = 86,
	W = 87,
	X = 88,
	Y = 89,
	Z = 90,
	LeftBracket = 91,
	BackSlash = 92,
	RightBracket = 93,
	GraveAccent = 96,
	World1 = 161,
	World2 = 162,
	Escape = 256,
	Enter = 257,
	Tab = 258,
	Backspace = 259,
	Insert = 260,
	Delete = 261,
	Right = 262,
	Left = 263,
	Down = 264,
	Up = 265,
	PageUp = 266,
	PageDown = 267,
	Home = 268,
	End = 269,
	CapsLock = 280,
	ScrollLock = 281,
	NumLock = 282,
	PrintScreen = 283,
	Pause = 284,
	F1 = 290,
	F2 = 291,
	F3 = 292,
	F4 = 293,
	F5 = 294,
	F6 = 295,
	F7 = 296,
	F8 = 297,
	F9 = 298,
	F10 = 299,
	F11 = 300,
	F12 = 301,
	F13 = 302,
	F14 = 303,
	F15 = 304,
	F16 = 305,
	F17 = 306,
	F18 = 307,
	F19 = 308,
	F20 = 309,
	F21 = 310,
	F22 = 311,
	F23 = 312,
	F24 = 313,
	F25 = 314,
	Keypad0 = 320,
	Keypad1 = 321,
	Keypad2 = 322,
	Keypad3 = 323,
	Keypad4 = 324,
	Keypad5 = 325,
	Keypad6 = 326,
	Keypad7 = 327,
	Keypad8 = 328,
	Keypad9 = 329,
	KeypadDecimal = 330,
	KeypadDivide = 331,
	KeypadMultiply = 332,
	KeypadSubtract = 333,
	KeypadAdd = 334,
	KeypadEnter = 335,
	KeypadEqual = 336,
	ShiftLeft = 340,
	ControlLeft = 341,
	AltLeft = 342,
	SuperLeft = 343,
	ShiftRight = 344,
	ControlRight = 345,
	AltRight = 346,
	SuperRight = 347,
	Menu = 348
}

public enum Button
{
	South,
	East,
	West,
	North,
	LeftBumper,
	RightBumper,
	Select,
	Start,
	Home,
	LeftStick,
	RightStick,
	DPadUp,
	DPadRight,
	DPadDown,
	DPadLeft,
	LeftTrigger,
	RightTrigger,
}

public enum InputState
{
	Down,
	Pressed,
	Up,
}
