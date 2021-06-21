using DudeiNoise;
using Unity.Collections;
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
            CheckNoiseMapUpToDate();
            terrainRenderer.DisplayMesh(GenerateTerrainMesh(), GenerateTerrainTexture(), tempDefinition.ChunkSize);
        }
        
        private TerrainMeshData GenerateTerrainMesh()
        {
            int width = tempDefinition.ChunkResolution;
            int height = tempDefinition.ChunkResolution;
            
            float topLeftCornerX = (width - 1) / -2f;
            float topLeftCornerZ = (height - 1) / 2f;
            
            TerrainMeshData meshData = new TerrainMeshData(width, height);
            int vertexIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float currentHeight = tempDefinition.HeightCurve.Evaluate(cachedNoiseMap[x, y]) * tempDefinition.HeightRange;
                    meshData.vertices[vertexIndex] = new Vector3(topLeftCornerX + x,currentHeight ,topLeftCornerZ - y) + tempDefinition.TerrainOffset;
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

        private Texture2D GenerateTerrainTexture()
        {
            int width = tempDefinition.ChunkResolution;
            int height = tempDefinition.ChunkResolution;
            
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            NativeArray<Color32> textureArray = texture.GetRawTextureData<Color32>();
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int i = 0; i < tempDefinition.TerrainLayers.Length; i++)
                    {
                        if (cachedNoiseMap[x,y] < tempDefinition.TerrainLayers[i].height)
                        {
                            textureArray[y * height + x] = tempDefinition.TerrainLayers[i].terrainColor;
                            break;
                        }
                    }
                }
            }

            texture.Apply();
            
            return texture;
        }
        
        private void CheckNoiseMapUpToDate()
        {
            int width = tempDefinition.ChunkResolution;
            int height = tempDefinition.ChunkResolution;

            if (width * height != cachedNoiseMap.Length)
            {
                Debug.LogWarning("Had to regenerate noise map!");
                UpdateNoiseMap();
            }
        }
        
        private void UpdateNoiseMap()
        {
            if (cachedNoiseMap == null || 
                cachedNoiseMap.GetLength(0) != tempDefinition.ChunkResolution||
                cachedNoiseMap.GetLength(1) != tempDefinition.ChunkResolution)
            {
                cachedNoiseMap = new float[tempDefinition.ChunkResolution , tempDefinition.ChunkResolution];
            }
            
            Noise.GenerateNoiseMap(ref cachedNoiseMap, tempDefinition.NoiseSettings);
        }
    }
}

