using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Procedural
{
	public partial class TerrainGenerator
	{
		private struct GenerateMeshJob : IJobParallelFor
		{
			[ReadOnly]
			public int fullMeshResolution;
			[ReadOnly]
			public int simplifiedMeshResolution;
			[ReadOnly]
			public float3 meshOffset;
			[ReadOnly]
			public float heightRange;
			
			[ReadOnly]
			public NativeArray<Color> noiseMap;
            
			[WriteOnly]
			public NativeArray<float3> vertices;
            
			[WriteOnly]
			public NativeArray<float2> uvs;
			
			[WriteOnly]
			public NativeArray<int> triangles;
			
			public void Execute(int index)
			{
				int indexModulo = index % 6;
				
				int triangleIndex = (int)math.floor(index / 6.0f);

				int vertexIndex = -1;
				
				switch (indexModulo)
				{
					case 0:		
						vertexIndex = triangleIndex;
						break;
					case 1:
						vertexIndex = triangleIndex + simplifiedMeshResolution + 1;
						break;
					case 2:
						vertexIndex = triangleIndex + simplifiedMeshResolution;
						break;
					case 3:
						vertexIndex = triangleIndex + simplifiedMeshResolution + 1;
						break;
					case 4:
						vertexIndex = triangleIndex;
						break;
					case 5:
						vertexIndex = triangleIndex + 1;
						break;
				}
				
				int x = vertexIndex % simplifiedMeshResolution;
				int y = (int) math.floor(vertexIndex / (float) simplifiedMeshResolution);
                
				float topLeftCornerX = (simplifiedMeshResolution - 1) / -2f;
				float topLeftCornerZ = (simplifiedMeshResolution - 1) / 2f;
                
				float currentHeight = noiseMap[vertexIndex].r * heightRange;
                
				vertices[index] = new float3(topLeftCornerX + x, currentHeight, topLeftCornerZ - y) + meshOffset;
                
				uvs[index] = new float2(x / (float) fullMeshResolution, y / (float) fullMeshResolution);
				
				triangles[index] = index;
			}
		}
	}
}