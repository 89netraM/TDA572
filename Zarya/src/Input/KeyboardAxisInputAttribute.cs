namespace Zarya.Input;

/// <summary>
/// Attribute for mapping two keyboard keys to an input action as an axis.
/// </summary>
public class KeyboardAxisInputAttribute : AxisInputAttribute
{
	/// <summary>
	/// The keyboard key used for the negative value.
	/// </summary>
	public Key Negative { get; }
	/// <summary>
	/// The keyboard key used for the positive value.
	/// </summary>
	public Key Positive { get; }

	/// <param name="negative">The keyboard key used for the negative value.</param>
	/// <param name="positive">The keyboard key used for the positive value.</param>
	public KeyboardAxisInputAttribute(Key negative, Key positive) =>
		(Negative, Positive) = (negative, positive);

	public override float GetValue(IInputManager inputManager, int _) =>
		(inputManager.IsKeyPressed(Negative), inputManager.IsKeyPressed(Positive)) switch
		{
			(true, false) => -1.0f,
			(false, true) => 1.0f,
			_ => 0.0f,
		};
}
