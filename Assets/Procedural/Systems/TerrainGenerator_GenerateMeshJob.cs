using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Procedural
{
    public partial class TerrainGenerator
    {
        private struct GenerateMeshJob : IJobParallelFor
        {
            [ReadOnly] public int size;
            [ReadOnly] public int lod;
            
            [ReadOnly] public NativeArray<float3> vertices;
            [ReadOnly] public NativeArray<int> triangles;
            [ReadOnly] public NativeArray<float2> uvs;
            
            private int traingleIndex;
            
            public void AddTriangle(int a, int b, int c)
            {
                triangles[traingleIndex] = a;
                triangles[traingleIndex + 1] = b;
                triangles[traingleIndex + 2] = c;

                traingleIndex += 3;
            }

            public void ApplyFlatShading()
            {
                NativeArray<float3> flatShadedVertices = new NativeArray<float3>(triangles.Length, Allocator.TempJob);
                NativeArray<float2> flatShadedUV = new NativeArray<float2>(triangles.Length, Allocator.TempJob);

                for (int i = 0; i < triangles.Length; i++)
                {
                    flatShadedVertices[i] = vertices[triangles[i]];
                    flatShadedUV[i] = uvs[triangles[i]];
                    triangles[i] = i;
                }

                vertices = flatShadedVertices;
                uvs = flatShadedUV;
            }
            
            
            
            public void Execute(int index)
            {
                float topLeftCornerX = (size - 1) / -2f;
                float topLeftCornerZ = (size - 1) / 2f;

                int meshSimplificationStep = lod == 0 ? 1 : lod * 2;
                int meshResolution = (size - 1) / meshSimplificationStep + 1;

                
                
            }
        }
    }
}