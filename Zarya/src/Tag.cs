using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Zarya;

/// <summary>
/// A readonly interface for string tags related to an entity.
/// </summary>
public interface ITag
{
	/// <summary>
	/// Checks if the tag is present on the entity.
	/// </summary>
	bool HasTag(string tag);
}

/// <summary>
/// A tag manager that can add and remove tags from an entity.
/// </summary>
public class TagManager : ITag
{
	private readonly ISet<string> tags = new HashSet<string>();

	/// <summary>
	/// Adds a tag to the entity.
	/// </summary>
	public void AddTag(string tag) => tags.Add(tag);

	/// <summary>
	/// Adds multiple tags to the entity.
	/// </summary>
	public void AddTag(params string[] tag) => tags.UnionWith(tag);

	/// <summary>
	/// Removes a tag from the entity.
	/// </summary>
	public void RemoveTag(string tag) => tags.Remove(tag);

	/// <inheritdoc />
	public bool HasTag(string tag) => tags.Contains(tag);
}

public static class TagManagerGameBuilderExtensions
{
	/// <summary>
	/// Adds <see cref="TagManager"/> and <see cref="ITag"/> capabilities to the game.
	/// </summary>
	public static GameBuilder AddTagManager(this GameBuilder gameBuilder)
	{
		gameBuilder.Services.AddScoped<TagManager, TagManager>();
		gameBuilder.Services.AddScoped<ITag>(sp => sp.GetRequiredService<TagManager>());
		return gameBuilder;
	}
}
