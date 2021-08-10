using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Procedural
{
    public partial class TerrainGenerator
    {
        private struct GenerateVerticesJob : IJobParallelFor
        {
            [ReadOnly]
            public int fullMeshResolution;
            [ReadOnly]
            public int simplifiedMeshResolution;
            [ReadOnly]
            public float heightRange;
            [ReadOnly]
            public float3 meshOffset;
            
            [ReadOnly]
            public NativeArray<Color> noiseMap;
            
            [WriteOnly]
            public NativeArray<float3> vertices;
            
            [WriteOnly]
            public NativeArray<float2> uvs;
            
            public void Execute(int index)
            {
                int x = index % simplifiedMeshResolution;
                int y = (int) math.floor(index / (float) simplifiedMeshResolution);
                
                float topLeftCornerX = (simplifiedMeshResolution - 1) / -2f;
                float topLeftCornerZ = (simplifiedMeshResolution - 1) / 2f;
                
                float currentHeight = noiseMap[index].r * heightRange;
                
                vertices[index] = new float3(topLeftCornerX + x, currentHeight, topLeftCornerZ - y) + meshOffset;
                
                uvs[index] = new float2(x / (float) fullMeshResolution, y / (float) fullMeshResolution);
            }
        }
    }
}