using DudeiNoise;
using UnityEngine;

namespace Procedural
{
    public partial class TerrainGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("DEBUG")]
        private TerrainDefinition tempDefinition = null;

        [SerializeField] 
        private TerrainRenderer terrainRenderer = null;
        
        private float[,] cachedNoiseMap = null;
        
        private void GenerateAndDisplayTerrain()
        {
            terrainRenderer.DisplayMesh(GenerateTerrainMesh(),null, tempDefinition.ChunkSize);
        }
        
        private TerrainMeshData GenerateTerrainMesh()
        {
            int width = tempDefinition.TextureSettings.resolution;
            int height = tempDefinition.TextureSettings.resolution;

            if (width * height != cachedNoiseMap.Length)
            {
                Debug.LogWarning("Had to regenerate noise map before generating terrain!");
                UpdateNoiseMap();
            }

            float topLeftCornerX = (width - 1) / -2f;
            float topLeftCornerZ = (height - 1) / 2f;
            
            TerrainMeshData meshData = new TerrainMeshData(width, height);
            int vertexIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftCornerX + x,cachedNoiseMap[x,y] * tempDefinition.HeightRange,topLeftCornerZ - y);
                    meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                    
                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex,vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
        
        private void UpdateNoiseMap()
        {
            if (cachedNoiseMap == null || 
                cachedNoiseMap.GetLength(0) != tempDefinition.TextureSettings.resolution ||
                cachedNoiseMap.GetLength(1) != tempDefinition.TextureSettings.resolution)
            {
                cachedNoiseMap = new float[tempDefinition.TextureSettings.resolution , tempDefinition.TextureSettings.resolution];
            }
            
            Noise.GenerateNoiseMap(ref cachedNoiseMap, tempDefinition.NoiseSettings);
        }
    }
}

