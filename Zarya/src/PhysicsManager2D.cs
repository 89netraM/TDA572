using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Extensions.DependencyInjection;

namespace Zarya;

/// <summary>
/// Manages the physics for 2D entities. Provides update events that are called every physics frame.
/// </summary>
public class PhysicsManager2D : IDisposable
{
	private readonly IGameManager gameManager;
	private readonly IList<PhysicsBody2D> bodies = new List<PhysicsBody2D>();
	private readonly ISet<CollisionRecord> previousCollissions = new HashSet<CollisionRecord>();

	/// <summary>
	/// The time in seconds between physics updates.
	/// </summary>
	public float DeltaTime { get; } = 1.0f / 60.0f;
	private float previousUpdateTime = 0.0f;

	/// <summary>
	/// An event called at the start of every physics update.
	/// </summary>
	public event Action? BeforeUpdate;

	/// <summary>
	/// An event called at the end of every physics update.
	/// </summary>
	public event Action? AfterUpdate;

	public PhysicsManager2D(IGameManager gameManager)
	{
		this.gameManager = gameManager;
		this.gameManager.Update += Update;
	}

	/// <summary>
	/// Adds a body to the physics manager.
	/// </summary>
	public void AddBody(PhysicsBody2D body) =>
		bodies.Add(body);

	/// <summary>
	/// Removes a body from the physics manager.
	/// </summary>
	public void RemoveBody(PhysicsBody2D body) =>
		bodies.Remove(body);

	private void Update(float deltaTime)
	{
		if (previousUpdateTime + DeltaTime < gameManager.Time)
		{
			previousUpdateTime = gameManager.Time;

			BeforeUpdate?.Invoke();

			foreach (var body in bodies)
			{
				body.Update();
			}

			var collisions = CheckCollisions();

			foreach (var coll in previousCollissions)
			{
				if (collisions.TryGetValue(coll, out var cImpulse))
				{
					var (c, impulse) = cImpulse;
					c.BodyA.TriggerCollisionStay(c.BodyB, impulse);
					c.BodyB.TriggerCollisionStay(c.BodyA, -impulse);
				}
				else
				{
					coll.BodyA.TriggerCollisionExit(coll.BodyB);
					coll.BodyB.TriggerCollisionExit(coll.BodyA);
				}
			}

			foreach (var (coll, impulse) in collisions.Values)
			{
				if (!previousCollissions.Contains(coll))
				{
					coll.BodyA.TriggerCollisionEnter(coll.BodyB, impulse);
					coll.BodyB.TriggerCollisionEnter(coll.BodyA, -impulse);
				}
			}

			previousCollissions.Clear();
			previousCollissions.UnionWith(collisions.Keys);

			AfterUpdate?.Invoke();
		}
	}

	private IDictionary<CollisionRecord, (CollisionRecord, Vector2)> CheckCollisions()
	{
		var possibleCollisions = BroadPass();

		var collisions = new Dictionary<CollisionRecord, (CollisionRecord, Vector2)>();
		foreach (var coll in possibleCollisions)
		{
			var possibleImpulse = CalculateCollision(coll);
			if (possibleImpulse is Vector2 impulse)
			{
				if (!coll.BodyA.IsPassThrough && !coll.BodyB.IsPassThrough)
				{
					var massProp = coll.BodyA.Mass / (coll.BodyA.Mass + coll.BodyB.Mass);
					coll.BodyA.ApplyCollision(coll.BodyB, massProp, impulse);
					coll.BodyB.ApplyCollision(coll.BodyA, 1.0f - massProp, -impulse);
				}

				collisions.Add(coll, (coll, impulse));
			}
		}
		return collisions;
	}

	private IList<CollisionRecord> BroadPass()
	{
		var aabbOrdered = new SortedList<AABB2D, PhysicsBody2D>(bodies.ToDictionary(b => b.CalculateAABB(), b => b));

		var possibleCollisions = new List<CollisionRecord>();

		var activeAABBs = new LinkedList<int>();
		var aabbsToRemove = new List<int>();
		for (int i = 0; i < aabbOrdered.Count; i++)
		{
			foreach (int j in activeAABBs)
			{
				if (aabbOrdered.Keys[i].Min.X >= aabbOrdered.Keys[j].Max.X)
				{
					aabbsToRemove.Add(j);
				}
				else
				{
					possibleCollisions.Add(new(aabbOrdered.Values[i], aabbOrdered.Values[j]));
				}
			}

			for (int j = aabbsToRemove.Count - 1; j >= 0; j--)
			{
				activeAABBs.Remove(aabbsToRemove[j]);
			}

			activeAABBs.AddLast(i);
		}

		return possibleCollisions;
	}

	private Vector2? CalculateCollision(CollisionRecord coll)
	{
		foreach (var colliderA in coll.BodyA.Colliders)
		{
			foreach (var colliderB in coll.BodyB.Colliders)
			{
				if (colliderA.CheckCollision(colliderB) is Vector2 impulse)
				{
					return impulse;
				}
			}
		}

		return null;
	}

	public void Dispose()
	{
		foreach (var body in bodies)
		{
			body.Dispose();
		}
	}
}

class CollisionRecord : IEquatable<CollisionRecord>
{
	public PhysicsBody2D BodyA { get; }
	public PhysicsBody2D BodyB { get; }

	public CollisionRecord(PhysicsBody2D bodyA, PhysicsBody2D bodyB) =>
		(BodyA, BodyB) = (bodyA, bodyB);

	public override bool Equals(object? other) =>
		other is CollisionRecord o && Equals(o);

	public bool Equals(CollisionRecord? other) =>
		other is CollisionRecord o &&
			(BodyA == o.BodyA && BodyB == o.BodyB ||
			BodyA == o.BodyB && BodyB == o.BodyA);

	public override int GetHashCode() =>
		BodyA.GetHashCode() ^ BodyB.GetHashCode();

	public static bool operator ==(CollisionRecord left, CollisionRecord right) =>
		left.Equals(right);

	public static bool operator !=(CollisionRecord left, CollisionRecord right) =>
		!left.Equals(right);
}

public static class PhysicsManager2DGameBuilderExtensions
{
	/// <summary>
	/// Adds the default 2D physics system to the game. This includes <see cref="Transform2D"/>, <see cref="PhysicsBody2D"/>, and <see cref="PhysicsManager2D"/>.
	/// </summary>
	public static GameBuilder AddDefault2DPhysics(this GameBuilder gameBuilder)
	{
		gameBuilder.Services.AddScoped<Transform2D>();
		gameBuilder.Services.AddScoped<PhysicsBody2D>();
		gameBuilder.Services.AddSingleton<PhysicsManager2D>();
		return gameBuilder;
	}
}
