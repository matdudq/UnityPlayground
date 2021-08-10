using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Procedural
{
	public partial class TerrainGenerator
	{
		[BurstCompile]
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

				int quadIndex = (int)math.floor(index / 6.0f);
				
				int stripeIndex = (int)math.floor(quadIndex / (float)(simplifiedMeshResolution-1));
				
				int vertexIndex = -1;
				
				switch (indexModulo)
				{
					case 0:		
						vertexIndex = quadIndex + stripeIndex;
						break;
					case 1:
						vertexIndex = quadIndex + stripeIndex + simplifiedMeshResolution + 1;
						break;
					case 2:
						vertexIndex = quadIndex + stripeIndex + simplifiedMeshResolution;
						break;
					case 3:
						vertexIndex = quadIndex + stripeIndex + simplifiedMeshResolution + 1;
						break;
					case 4:
						vertexIndex = quadIndex + stripeIndex;
						break;
					case 5:
						vertexIndex = quadIndex + stripeIndex + 1;
						break;
				}
				
				int x = vertexIndex % simplifiedMeshResolution;
				int y = (int) math.floor(vertexIndex / (float) simplifiedMeshResolution);

				float xRatio = x / (float) simplifiedMeshResolution;
				float yRatio = y / (float) simplifiedMeshResolution;
				
				float topLeftCornerX = (fullMeshResolution - 1) / -2f;
				float topLeftCornerZ = (fullMeshResolution - 1) / 2f;
                
				float currentHeight = noiseMap[x + simplifiedMeshResolution * y].r * heightRange;
                
				vertices[index] = new float3(topLeftCornerX + xRatio * fullMeshResolution, currentHeight, topLeftCornerZ - yRatio * fullMeshResolution) + meshOffset;
                
				uvs[index] = new float2(xRatio, yRatio);
				
				triangles[index] = index;
			}
		}
	}
}