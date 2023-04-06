namespace Zarya.RayTracer;

public readonly struct RayTracerObject
{
	public readonly float3 Position;
	public readonly float Radius;

	public readonly float3 Albedo;
	public readonly float3 Emissive;

	public RayTracerObject(float3 position, float radius, float3 albedo, float3 emissive)
	{
		Position = position;
		Radius = radius;
		Albedo = albedo;
		Emissive = emissive;
	}
}
