using System;
using ComputeSharp;

namespace Zarya.RayTracer;

[EmbeddedBytecode(8, 8, 1)]
readonly partial struct Shader : IPixelShader<float4>
{
	private const float AmbientLight = 0.1f;

	private readonly float time;
	private readonly float cameraDistance;
	private readonly float aspectRatio;
	private readonly ReadOnlyBuffer<RayTracerObject> objects;
	private readonly int cameraRayCount;
	private readonly int lightRayCount;
	private readonly float3 ambientLight;

	public Shader(float time, float fov, float aspectRatio, ReadOnlyBuffer<RayTracerObject> objects, float3? ambientLight = null, int cameraRayCount = 50, int lightRayCount = 50)
	{
		this.time = time;
		this.cameraDistance = 1.0f / MathF.Tan(fov * 0.5f * MathF.PI / 180.0f);
		this.aspectRatio = aspectRatio;
		this.objects = objects;
		this.cameraRayCount = cameraRayCount;
		this.lightRayCount = lightRayCount;
		this.ambientLight = ambientLight ?? new(AmbientLight, AmbientLight, AmbientLight);
	}

	public float4 Execute()
	{
		var rngState = RngSeed(ThreadIds.XY);

		float3 rayTarget = new(ThreadIds.Normalized.XY * 2.0f - 1.0f, cameraDistance);
		rayTarget.Y /= aspectRatio;

		float3 color = new(0.0f, 0.0f, 0.0f);
		for (int i = 0; i < cameraRayCount; i++)
		{
			float3 rayOrigin = new(RandUnitVector2(ref rngState) * 0.001f, 0.0f);
			float3 rayDir = Hlsl.Normalize(rayTarget - rayOrigin);
			color += GetColorForRay(rayOrigin, rayDir, ref rngState);
		}
		return new(color / cameraRayCount, 1.0f);
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

	private float2 RandUnitVector2(ref uint rngState)
	{
		float a = RandFloat(ref rngState) * 2.0f * MathF.PI;
		return new(Hlsl.Cos(a), Hlsl.Sin(a));
	}

	private float3 RandUnitVector3(ref uint rngState)
	{
		float z = RandFloat(ref rngState) * 2.0f - 1.0f;
		float a = RandFloat(ref rngState) * 2.0f * MathF.PI;
		float r = Hlsl.Sqrt(1.0f - z * z);
		return new(r * Hlsl.Cos(a), r * Hlsl.Sin(a), z);
	}

	private float3 GetColorForRay(float3 rayPos, float3 rayDir, ref uint rngState)
	{
		float3 color = new(0.0f, 0.0f, 0.0f);

		if (!IntersectObjects(rayPos, rayDir, out HitInfo hitInfo))
		{
			return color;
		}

		var hitObject = objects[hitInfo.ObjectIndex];
		if (Hlsl.Any(hitObject.Emissive))
		{
			return hitObject.Emissive;
		}
		color += hitObject.Albedo * ambientLight;

		for (int i = 0; i < objects.Length; i++)
		{
			var lightObject = objects[i];
			if (!Hlsl.Any(lightObject.Emissive))
			{
				continue;
			}
			float3 lightRayPos = hitInfo.Position + hitInfo.Normal * 0.001f;
			for (int l = 0; l < lightRayCount; l++)
			{
				float3 lightRayTarget = lightObject.Position + RandUnitVector3(ref rngState) * RandFloat(ref rngState) * lightObject.Radius;
				float3 lightRayDir = Hlsl.Normalize(lightRayTarget - lightRayPos);
				if (IntersectObjects(lightRayPos, lightRayDir, out HitInfo lightHitInfo) && lightHitInfo.ObjectIndex == i)
				{
					float3 lightColor = lightObject.Emissive;
					float3 lightIntensity = lightColor * Hlsl.Max(Hlsl.Dot(hitInfo.Normal, lightRayDir), 0.0f);
					color += hitObject.Albedo * lightIntensity / lightRayCount;
				}
			}
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