using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Procedural
{
	public partial class TerrainGenerator
	{
		private struct ApplyFlatShadingJob : IJobParallelFor
		{
			[ReadOnly]
			public NativeArray<float3> vertices;
			[ReadOnly]
			public NativeArray<float2> uvs;
			
			[WriteOnly]
			public NativeArray<float3> flatShadedVertices;
			[WriteOnly]
			public NativeArray<float2> flatShadedUV;

			public NativeArray<int> triangles;
			
			public void Execute(int index)
			{
				flatShadedVertices[index] = vertices[triangles[index]];
				flatShadedUV[index] = uvs[triangles[index]];
				triangles[index] = index;
			}
		}
	}
}