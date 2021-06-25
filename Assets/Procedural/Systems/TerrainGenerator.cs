using DudeiNoise;
using Unity.Collections;
using UnityEngine;
using Utilities;

namespace Procedural
{
    public partial class TerrainGenerator : SingletonMonoBehaviour<TerrainGenerator>
    {
        [SerializeField, Tooltip("DEBUG")]
        private TerrainDefinition tempDefinition = null;

        [SerializeField] 
        private TerrainRenderer terrainRenderer = null;
        
        private float[,] cachedNoiseMap = null;

        protected override void Awake()
        {
            base.Awake();
            UpdateNoiseMap();
        }
        
        private void GenerateAndDisplayTerrain()
        {
            CheckNoiseMapUpToDate();
            terrainRenderer.DisplayMesh(GenerateTerrainMesh(), GenerateTerrainTexture(), tempDefinition.ChunkSize);
        } 
        
        public TerrainMeshData GenerateTerrainMesh()
        {
            int width = TerrainDefinition.MAP_CHUNK_SIZE;
            int height = TerrainDefinition.MAP_CHUNK_SIZE;
            
            float topLeftCornerX = (width - 1) / -2f;
            float topLeftCornerZ = (height - 1) / 2f;

            int meshSimplificationStep = tempDefinition.LevelOfDetails == 0 ? 1 : tempDefinition.LevelOfDetails * 2;
            int meshResolution = (width - 1) / meshSimplificationStep +1;
            
            TerrainMeshData meshData = new TerrainMeshData(width, height);
            int vertexIndex = 0;

            for (int y = 0; y < height; y += meshSimplificationStep)
            {
                for (int x = 0; x < width; x += meshSimplificationStep)
                {
                    float currentHeight = tempDefinition.HeightCurve.Evaluate(cachedNoiseMap[x, y]) * tempDefinition.HeightRange;
                    meshData.vertices[vertexIndex] = new Vector3(topLeftCornerX + x,currentHeight ,topLeftCornerZ - y) + tempDefinition.TerrainOffset;
                    meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                    
                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex,vertexIndex + meshResolution + 1, vertexIndex + meshResolution);
                        meshData.AddTriangle(vertexIndex + meshResolution + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }

        public Texture2D GenerateTerrainTexture()
        {
            int width =  TerrainDefinition.MAP_CHUNK_SIZE;
            int height =  TerrainDefinition.MAP_CHUNK_SIZE;
            
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
            int width = TerrainDefinition.MAP_CHUNK_SIZE;
            int height = TerrainDefinition.MAP_CHUNK_SIZE;

            if (width * height != cachedNoiseMap.Length)
            {
                Debug.LogWarning("Had to regenerate noise map!");
                UpdateNoiseMap();
            }
        }
        
        private void UpdateNoiseMap()
        {
            if (cachedNoiseMap == null || 
                cachedNoiseMap.GetLength(0) != TerrainDefinition.MAP_CHUNK_SIZE||
                cachedNoiseMap.GetLength(1) != TerrainDefinition.MAP_CHUNK_SIZE)
            {
                cachedNoiseMap = new float[TerrainDefinition.MAP_CHUNK_SIZE , TerrainDefinition.MAP_CHUNK_SIZE];
            }
            
            Noise.GenerateNoiseMap(ref cachedNoiseMap, tempDefinition.NoiseSettings);
        }
    }
}

