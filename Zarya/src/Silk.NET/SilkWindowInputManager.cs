using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Silk.NET.Input;
using SilkKey = Silk.NET.Input.Key;
using SilkMouseButton = Silk.NET.Input.MouseButton;
using SilkButton = Silk.NET.Input.Button;

namespace Zarya.Silk.NET;

public partial class SilkWindow : IInputManager
{
	private IInputContext? inputContext = null;

	private readonly ICollection<IKeyboard> keyboards = new List<IKeyboard>();
	private readonly ICollection<IMouse> mice = new List<IMouse>();
	private readonly IDictionary<int, IGamepad> gamepads = new Dictionary<int, IGamepad>();

	private readonly IDictionary<Key, InputState> keyStates = new Dictionary<Key, InputState>();
	private readonly IDictionary<MouseButton, InputState> mouseButtonStates = new Dictionary<MouseButton, InputState>();
	private readonly IDictionary<(Button, int), InputState> buttonStates = new Dictionary<(Button, int), InputState>();

	private void InitializeInput()
	{
		inputContext = window!.CreateInput();
		inputContext.ConnectionChanged += OnConnectionChanged;

		foreach (var keyboard in inputContext.Keyboards)
		{
			if (keyboard.IsConnected)
			{
				RegisterKeyboard(keyboard);
			}
		}

		foreach (var mouse in inputContext.Mice)
		{
			if (mouse.IsConnected)
			{
				RegisterMouse(mouse);
			}
		}

		foreach (var gamepad in inputContext.Gamepads)
		{
			if (gamepad.IsConnected)
			{
				RegisterGamepad(gamepad);
			}
		}
	}

	private void OnConnectionChanged(IInputDevice device, bool connected)
	{
		if (device is IKeyboard keyboard)
		{
			if (connected)
			{
				RegisterKeyboard(keyboard);
			}
			else
			{
				UnregisterKeyboard(keyboard);
			}
		}
		else if (device is IMouse mouse)
		{
			if (connected)
			{
				RegisterMouse(mouse);
			}
			else
			{
				UnregisterMouse(mouse);
			}
		}
		else if (device is IGamepad gamepad)
		{
			if (connected)
			{
				RegisterGamepad(gamepad);
			}
			else
			{
				UnregisterGamepad(gamepad);
			}
		}
	}

	private void RegisterKeyboard(IKeyboard keyboard)
	{
		keyboard.KeyDown += OnKeyDown;
		keyboard.KeyUp += OnKeyUp;
		keyboards.Add(keyboard);
	}

	private void UnregisterKeyboard(IKeyboard keyboard)
	{
		keyboard.KeyDown -= OnKeyDown;
		keyboard.KeyUp -= OnKeyUp;
		keyboards.Remove(keyboard);
	}

	private void RegisterMouse(IMouse mouse)
	{
		mouse.MouseDown += OnMouseButtonDown;
		mouse.MouseUp += OnMouseButtonUp;
		mice.Add(mouse);
	}

	private void UnregisterMouse(IMouse mouse)
	{
		mouse.MouseDown -= OnMouseButtonDown;
		mouse.MouseUp -= OnMouseButtonUp;
		mice.Remove(mouse);
	}

	private void RegisterGamepad(IGamepad gamepad)
	{
		gamepad.ButtonDown += OnGamepadButtonDown;
		gamepad.ButtonUp += OnGamepadButtonUp;
		gamepad.Deadzone = new Deadzone(0.2f, DeadzoneMethod.Traditional);
		gamepads.Add(gamepad.Index, gamepad);
	}

	private void UnregisterGamepad(IGamepad gamepad)
	{
		gamepad.ButtonDown -= OnGamepadButtonDown;
		gamepad.ButtonUp -= OnGamepadButtonUp;
		gamepads.Remove(gamepad.Index);
	}

	private void OnKeyDown(IKeyboard keyboard, SilkKey key, int ascii)
	{
		keyStates[(Key)key] = InputState.Down;
	}

	private void OnKeyUp(IKeyboard keyboard, SilkKey key, int ascii)
	{
		keyStates[(Key)key] = InputState.Up;
	}

	private void OnMouseButtonDown(IMouse mouse, SilkMouseButton button)
	{
		mouseButtonStates[(MouseButton)button] = InputState.Down;
	}

	private void OnMouseButtonUp(IMouse mouse, SilkMouseButton button)
	{
		mouseButtonStates[(MouseButton)button] = InputState.Up;
	}

	private void OnGamepadButtonDown(IGamepad gamepad, SilkButton button)
	{
		buttonStates[((Button)button.Name, gamepad.Index)] = InputState.Down;
	}

	private void OnGamepadButtonUp(IGamepad gamepad, SilkButton button)
	{
		buttonStates[((Button)button.Name, gamepad.Index)] = InputState.Up;
	}

	private void UpdateInput()
	{
		foreach (var key in keyStates.Keys)
		{
			if (keyStates[key] == InputState.Down)
			{
				keyStates[key] = InputState.Pressed;
			}
			else if (keyStates[key] == InputState.Up)
			{
				keyStates.Remove(key);
			}
		}

		foreach (var mouseButton in mouseButtonStates.Keys)
		{
			if (mouseButtonStates[mouseButton] == InputState.Down)
			{
				mouseButtonStates[mouseButton] = InputState.Pressed;
			}
			else if (mouseButtonStates[mouseButton] == InputState.Up)
			{
				mouseButtonStates.Remove(mouseButton);
			}
		}

		foreach (var button in buttonStates.Keys)
		{
			if (buttonStates[button] == InputState.Down)
			{
				buttonStates[button] = InputState.Pressed;
			}
			else if (buttonStates[button] == InputState.Up)
			{
				buttonStates.Remove(button);
			}
		}
	}

	/// <inheritdoc/>
	public bool IsKeyDown(Key key) =>
		keyStates.TryGetValue(key, out var state) && state == InputState.Down;

	/// <inheritdoc/>
	public bool IsKeyPressed(Key key) =>
		keyStates.TryGetValue(key, out var state) && state == InputState.Pressed;

	/// <inheritdoc/>
	public bool IsKeyUp(Key key) =>
		keyStates.TryGetValue(key, out var state) && state == InputState.Up;

	/// <inheritdoc/>
	public Vector2? GetMousePosition()
	{
		if (inputContext?.Mice.FirstOrDefault() is IMouse mouse)
		{
			return mouse.Position;
		}

		return null;
	}

	/// <inheritdoc/>
	public bool IsMouseButtonDown(MouseButton button) =>
		mouseButtonStates.TryGetValue(button, out var state) && state == InputState.Down;

	/// <inheritdoc/>
	public bool IsMouseButtonPressed(MouseButton button) =>
		mouseButtonStates.TryGetValue(button, out var state) && state == InputState.Pressed;

	/// <inheritdoc/>
	public bool IsMouseButtonUp(MouseButton button) =>
		mouseButtonStates.TryGetValue(button, out var state) && state == InputState.Up;

	/// <inheritdoc/>
	public bool IsButtonDown(Button button) =>
		gamepads.Keys.Any(index => IsButtonDown(button, index));

	/// <inheritdoc/>
	public bool IsButtonDown(Button button, int player) =>
		buttonStates.TryGetValue((button, player), out var state) && state == InputState.Down;

	/// <inheritdoc/>
	public bool IsButtonPressed(Button button) =>
		gamepads.Keys.Any(index => IsButtonPressed(button, index));

	/// <inheritdoc/>
	public bool IsButtonPressed(Button button, int player) =>
		buttonStates.TryGetValue((button, player), out var state) && state == InputState.Pressed;

	/// <inheritdoc/>
	public bool IsButtonUp(Button button) =>
		gamepads.Keys.Any(index => IsButtonUp(button, index));

	/// <inheritdoc/>
	public bool IsButtonUp(Button button, int player) =>
		buttonStates.TryGetValue((button, player), out var state) && state == InputState.Up;


	/// <inheritdoc/>
	public float GetGamepadAxis(GamepadAxis axis) =>
		gamepads.Keys.Select(index => GetGamepadAxis(axis, index))
			.FirstOrDefault(value => value != 0.0f);

	/// <inheritdoc/>
	public float GetGamepadAxis(GamepadAxis axis, int player) => axis switch
	{
		GamepadAxis.LeftStickX => gamepads.TryGetValue(player, out var gamepad) ? gamepad.Thumbsticks[0].X : 0.0f,
		GamepadAxis.LeftStickY => gamepads.TryGetValue(player, out var gamepad) ? -gamepad.Thumbsticks[0].Y : 0.0f,
		GamepadAxis.RightStickX => gamepads.TryGetValue(player, out var gamepad) ? gamepad.Thumbsticks[1].X : 0.0f,
		GamepadAxis.RightStickY => gamepads.TryGetValue(player, out var gamepad) ? -gamepad.Thumbsticks[1].Y : 0.0f,
		GamepadAxis.Trigger => gamepads.TryGetValue(player, out var gamepad) ? -(gamepad.Triggers[0].Position + 1.0f) / 2.0f + (gamepad.Triggers[1].Position + 1.0f) / 2.0f : 0.0f,
		_ => 0.0f,
	};

	public void DisposeInput()
	{
		foreach (var keyboard in keyboards)
		{
			UnregisterKeyboard(keyboard);
		}
		keyboards.Clear();
		keyStates.Clear();

		foreach (var mouse in mice)
		{
			UnregisterMouse(mouse);
		}
		mice.Clear();
		mouseButtonStates.Clear();

		foreach (var gamepad in gamepads.Values)
		{
			UnregisterGamepad(gamepad);
		}
		gamepads.Clear();
		buttonStates.Clear();
	}
}
