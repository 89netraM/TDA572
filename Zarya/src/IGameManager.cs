using System;
using System.Diagnostics.CodeAnalysis;

namespace Zarya;

/// <summary>
/// An interface that manages the update loop and entity lifecycle of a game.
/// </summary>
public interface IGameManager
{
	/// <summary>
	/// The current time of the game, in seconds.
	/// </summary>
	float Time { get; }

	/// <summary>
	/// Called when the game is initialized, or immediately if the game already is initialized.
	/// </summary>
	event Action? Initialize;
	/// <summary>
	/// Called once for each frame of the game. Providing the time since the last frame as a parameter.
	/// </summary>
	event Action<float>? Update;

	protected internal void Run();

	/// <summary>
	/// Creates a new entity of type <typeparamref name="T"/>.
	/// Provide the parameters to the constructor of the entity that cannot be provided by the system.
	/// </summary>
	T? Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object[] parameters) where T : class;
	/// <summary>
	/// Destroys the entity (or returns false if the entity does not exist), and disposes of all resources associated with it.
	/// </summary>
	bool Destroy<T>(T obj) where T : class;

	/// <summary>
	/// Terminates the game loop nad closes the game.
	/// </summary>
	void Close();
}
