using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Extensions.DependencyInjection;

namespace Zarya;

/// <summary>
/// A 2D physics enabled body that acts on an entity's <see cref="Transform2D"/> in conjunction with the <see cref="PhysicsManager2D"/>.
/// </summary>
public class PhysicsBody2D : IDisposable
{
	private readonly Transform2D transform;
	private readonly PhysicsManager2D physicsManager;
	private readonly IServiceProvider serviceProvider;

	/// <summary>
	/// The maximum angular velocity of the body, in units per seconds.
	/// </summary>
	public float MaxAngularVelocity { get; set; } = float.PositiveInfinity;
	private float angularVelocity = 0.0f;
	/// <summary>
	/// The angular velocity of the body, in units per second.
	/// </summary>
	public float AngularVelocity
	{
		get => angularVelocity;
		set
		{
			if (MathF.Abs(value) > MaxAngularVelocity)
			{
				angularVelocity = MathF.CopySign(MaxAngularVelocity, value);
			}
			else
			{
				angularVelocity = value;
			}
		}
	}
	/// <summary>
	/// The angular drag, in units per second per second.
	/// </summary>
	public float AngularDrag { get; set; } = 0.0f;

	/// <summary>
	/// The maximum linear velocity of the body, in units per second.
	/// </summary>
	public float MaxVelocity { get; set; } = float.PositiveInfinity;
	private Vector2 velocity = Vector2.Zero;
	/// <summary>
	/// The linear velocity of the body, in units per second.
	/// </summary>
	public Vector2 Velocity
	{
		get => velocity;
		set
		{
			velocity = value;
			if (velocity.LengthSquared() > MaxVelocity * MaxVelocity)
			{
				velocity = Vector2.Normalize(velocity) * MaxVelocity;
			}
		}
	}
	/// <summary>
	/// The linear drag, in units per second per second.
	/// </summary>
	public float Drag { get; set; } = 0.0f;

	/// <summary>
	/// A constant acceleration applied to the body, in units per second per second.
	public Vector2 ConstantAcceleration { get; set; } = Vector2.Zero;

	/// <summary>
	/// The mass of the body, in units.
	/// </summary>
	public float Mass { get; set; } = 1.0f;

	/// <summary>
	/// Can this body pass through other bodies?
	/// </summary>
	public bool IsPassThrough { get; set; } = false;

	/// <summary>
	/// Should this body be affected by physics?
	/// </summary>
	public bool IsKinematic { get; set; } = false;

	/// <summary>
	/// Does this body impart its forces on other bodies?
	/// </summary>
	public bool ImpartForces { get; set; } = true;

	/// <summary>
	/// Should this body stop immediately when it hits another body?
	/// </summary>
	public bool StopOnCollsion { get; set; } = false;

	private readonly List<ICollider2D> colliders = new List<ICollider2D>();
	/// <summary>
	/// The colliders that are attached to this body.
	/// </summary>
	public IReadOnlyList<ICollider2D> Colliders => colliders;

	/// <summary>
	/// Called when this body collides with another body. Provides the other body and the impact vector as a parameter.
	/// </summary>
	public event Action<PhysicsBody2D, Vector2>? CollisionEnter;
	/// <summary>
	/// Called for every physics update while this body collides with another body. Provides the other body and the impact vector as a parameter.
	/// </summary>
	public event Action<PhysicsBody2D, Vector2>? CollisionStay;
	/// <summary>
	/// Called when this body stops colliding with another body. Provides the other body as a parameter.
	/// </summary>
	public event Action<PhysicsBody2D>? CollisionExit;

	/// <summary>
	/// A means of sending messages to this entity.
	/// </summary>
	public IMessagePasser MessagePasser { get; }

	/// <summary>
	/// The tags associated with this body.
	/// </summary>
	public ITag Tag { get; }

	public PhysicsBody2D(Transform2D transform, PhysicsManager2D physicsManager, IMessagePasser messagePasser, ITag tag, IServiceProvider serviceProvider)
	{
		this.transform = transform;

		this.physicsManager = physicsManager;
		this.physicsManager.AddBody(this);

		MessagePasser = messagePasser;

		Tag = tag;

		this.serviceProvider = serviceProvider;
	}

	/// <summary>
	/// Adds a collider of type <typeparamref name="T"/> to this body.
	/// Provide the parameters to the constructor of the collider that cannot be provided by the system.
	/// </summary>
	public T? AddCollider<T>(params object[] parameters) where T : class, ICollider2D
	{
		var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
		if (obj is T)
		{
			colliders.Add(obj);
			return obj;
		}
		return null;
	}

	internal void Update()
	{
		if (!IsKinematic)
		{
			transform.Rotation += AngularVelocity * physicsManager.DeltaTime;
			if (Math.Abs(AngularVelocity) < AngularDrag)
			{
				AngularVelocity = 0.0f;
			}
			else
			{
				AngularVelocity -= Math.Sign(AngularVelocity) * AngularDrag * physicsManager.DeltaTime;
			}

			Velocity += ConstantAcceleration * physicsManager.DeltaTime;
			transform.Position += Velocity * physicsManager.DeltaTime;
			float velocityLength = Velocity.Length();
			if (velocityLength < Drag)
			{
				Velocity = Vector2.Zero;
			}
			else
			{
				Velocity -= Vector2.One * Drag * physicsManager.DeltaTime;
			}
		}
	}

	internal AABB2D CalculateAABB()
	{
		var min = transform.Position;
		var max = transform.Position;

		foreach (var collider in Colliders)
		{
			var (minCollider, maxCollider) = collider.CalculateAABB();

			min = Vector2.Min(min, minCollider);
			max = Vector2.Max(max, maxCollider);
		}

		return new AABB2D(min, max);
	}

	internal void ApplyCollision(PhysicsBody2D other, float massProportion, Vector2 impulse)
	{
		if (IsKinematic)
		{
			massProportion = 1.0f;
		}

		if (ImpartForces)
		{
			other.Velocity += Velocity * massProportion;
			Velocity *= 1.0f - massProportion;
		}

		if (StopOnCollsion)
		{
			Velocity = Vector2.Zero;
		}

		if (!other.IsKinematic)
		{
			other.transform.Position -= impulse * massProportion;
		}
	}

	internal void TriggerCollisionEnter(PhysicsBody2D other, Vector2 impulse) =>
		CollisionEnter?.Invoke(other, impulse);
	internal void TriggerCollisionStay(PhysicsBody2D other, Vector2 impulse) =>
		CollisionStay?.Invoke(other, impulse);
	internal void TriggerCollisionExit(PhysicsBody2D other) =>
		CollisionExit?.Invoke(other);

	public void Dispose()
	{
		physicsManager.RemoveBody(this);
		foreach (var collider in colliders)
		{
			if (collider is IDisposable disposableCollider)
			{
				disposableCollider.Dispose();
			}
		}
	}
}
