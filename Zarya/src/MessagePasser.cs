using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zarya;

/// <summary>
/// An interface with which messages can be sent to an entity.
/// </summary>
public interface IMessagePasser
{
	/// <summary>
	/// Sends a message to the entity.
	/// </summary>
	void SendMessage(object message);
}

/// <summary>
/// An interface from which a message can be received.
/// </summary>
public interface IMessageReceiver
{
	/// <summary>
	/// Listens for messages to this entity.
	/// </summary>
	event Action<object> Message;
}

class MessageMannager : IMessagePasser, IMessageReceiver
{
	public event Action<object>? Message;

	public void SendMessage(object message) =>
		Message?.Invoke(message);
}

public static class MessagePasserGameBuilderExtensions
{
	/// <summary>
	/// Adds <see cref="IMessagePasser"/> and <see cref="IMessageReceiver"/> capabilities to the game.
	/// </summary>
	public static GameBuilder AddMessagePasser(this GameBuilder gameBuilder)
	{
		gameBuilder.Services.AddScoped<MessageMannager, MessageMannager>();
		gameBuilder.Services.AddScoped<IMessagePasser>(sp => sp.GetRequiredService<MessageMannager>());
		gameBuilder.Services.AddScoped<IMessageReceiver>(sp => sp.GetRequiredService<MessageMannager>());
		return gameBuilder;
	}
}
