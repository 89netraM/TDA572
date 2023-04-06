using System;
using ComputeSharp;

namespace Zarya.RayTracer;

[EmbeddedBytecode(8, 8, 1)]
readonly partial struct Shader : IPixelShader<float4>
{
	private readonly float time;
	private readonly float cameraDistance;
	private readonly float aspectRatio;
	private readonly ReadOnlyBuffer<RayTracerObject> objects;
	private readonly int numberOfRays;
	private readonly int numberOfBounces;

	public Shader(float time, float fov, float aspectRatio, ReadOnlyBuffer<RayTracerObject> objects, int numberOfRays = 10, int numberOfBounces = 5)
	{
		this.time = time;
		this.cameraDistance = 1.0f / MathF.Tan(fov * 0.5f * MathF.PI / 180.0f);
		this.aspectRatio = aspectRatio;
		this.objects = objects;
		this.numberOfRays = numberOfRays;
		this.numberOfBounces = numberOfBounces;
	}

	public float4 Execute()
	{
		var rngState = RngSeed(ThreadIds.XY);

		float3 rayPosition = new(0.0f, 0.0f, 0.0f);
		float3 rayTarget = new(ThreadIds.Normalized.XY * 2.0f - 1.0f, cameraDistance);
		rayTarget.Y /= aspectRatio;
		float3 rayDir = Hlsl.Normalize(rayTarget - rayPosition);

		float3 color = new(0.0f, 0.0f, 0.0f);
		for (int i = 0; i < numberOfRays; i++)
		{
			color += GetColorForRay(rayPosition, rayDir, ref rngState);
		}
		return new(color / (float)numberOfRays, 1.0f);
	}

	private uint RngSeed(int2 xy) =>
		((uint)xy.X * 1973u + (uint)xy.Y * 9277u + (uint)(time * 26699u)) | 1u;

	private float RandFloat(ref uint rngState)
	{
		rngState = (rngState ^61u) ^ (rngState >> 16);
		rngState *= 9u;
		rngState = rngState ^ (rngState >> 4);
		rngState *= 0x27d4eb2du;
		rngState = rngState ^ (rngState >> 15);
		return (float)rngState / uint.MaxValue;
	}

	private float3 RandUnitVector(ref uint rngState)
	{
		float z = RandFloat(ref rngState) * 2.0f - 1.0f;
		float a = RandFloat(ref rngState) * 2.0f * MathF.PI;
		float r = Hlsl.Sqrt(1.0f - z * z);
		return new(r * Hlsl.Cos(a), r * Hlsl.Sin(a), z);
	}

	private float3 GetColorForRay(float3 startRayPos, float3 startRayDir, ref uint rngState)
	{
		float3 color = new(0.0f, 0.0f, 0.0f);
		float3 throughput = new(1.0f, 1.0f, 1.0f);
		float3 rayPos = startRayPos;
		float3 rayDir = startRayDir;

		for (int bounceIndex = 0; bounceIndex < numberOfBounces; bounceIndex++)
		{
			if (!IntersectObjects(rayPos, rayDir, out HitInfo hitInfo))
			{
				break;
			}

			rayPos = hitInfo.Position + hitInfo.Normal * 0.01f;
			rayDir = Hlsl.Normalize(hitInfo.Normal + RandUnitVector(ref rngState));

			var hitObject = objects[hitInfo.ObjectIndex];
			color += hitObject.Emissive * throughput;
			throughput *= hitObject.Albedo;
		}

		return color;
	}

	private bool IntersectObjects(float3 rayPosition, float3 rayDir, out HitInfo hitInfo)
	{
		hitInfo = default;
		float closestHit = float.MaxValue;
		for (int i = 0; i < objects.Length; i++)
		{
			if (IntersectSphere(rayPosition, rayDir, objects[i].Position, objects[i].Radius, out var hitTestInfo))
			{
				if (hitTestInfo.Distance < closestHit)
				{
					closestHit = hitTestInfo.Distance;
					hitInfo = hitTestInfo;
					hitInfo.ObjectIndex = i;
				}
			}
		}
		return closestHit < float.MaxValue;
	}

	private bool IntersectSphere(float3 rayPos, float3 rayDir, float3 spherePos, float sphereRadius, out HitInfo hitInfo)
	{
		hitInfo = default;

		float3 m = rayPos - spherePos;

		float b = Hlsl.Dot(m, rayDir);
		float c = Hlsl.Dot(m, m) - sphereRadius * sphereRadius;

		if (c > 0.0f && b > 0.0f)
		{
			return false;
		}

		float discr = b * b - c;

		if (discr < 0.0f)
		{
			return false;
		}

		bool fromInside = false;
		float dist = -b - Hlsl.Sqrt(discr);
		if (dist < 0.0f)
		{
			fromInside = true;
			dist = -b + Hlsl.Sqrt(discr);
		}

		if (dist > 0.001f)
		{
			hitInfo.Distance = dist;
			hitInfo.Position = rayPos + rayDir * dist;
			hitInfo.Normal = Hlsl.Normalize(hitInfo.Position - spherePos);
			if (fromInside)
			{
				hitInfo.Normal = -hitInfo.Normal;
			}
			return true;
		}

		return false;
	}
}

struct HitInfo
{
	public float Distance;
	public float3 Position;
	public float3 Normal;
	public int ObjectIndex;
}