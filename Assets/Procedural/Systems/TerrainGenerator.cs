using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DudeiNoise;
using Procedural.Utilities;
using Unity.Collections;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;

namespace Procedural
{
    public partial class TerrainGenerator : SingletonMonoBehaviour<TerrainGenerator>
    {
        [Header("Editor preview")]
        [SerializeField]
        private TerrainDefinition definition = null;

        [SerializeField]
        private TerrainRenderer terrainRenderer = null;
        
        private Queue<ThreadRequestData<TerrainData>> requestedTerrainQueue   = null;
        
        #region Unity methods

        protected override void Awake()
        {
            base.Awake();
            
            requestedTerrainQueue = new Queue<ThreadRequestData<TerrainData>>();
        }

        private void Update()
        {
            for (int i = 0; i < requestedTerrainQueue.Count; i++)
            {
                ProceedRequestedTerrains();
            }
        }

        #endregion Unity methods
        
        public void RequestTerrain(int lod, Vector2 tile, Action<TerrainData> onRequest = null)
        {
            ThreadStart threadStart = ThreadProcess;

            new Thread(threadStart).Start();

            void ThreadProcess()
            {
                requestedTerrainQueue.Enqueue(new ThreadRequestData<TerrainData>(onRequest,  Generate(lod, tile)));
            }
        }

        public TerrainData Generate(int lod, Vector2 tile)
        {
            float[,] terrainHeightMap = GenerateHeightMapForOrigin(tile);
            TerrainMeshData meshData = GenerateTerrainMesh(terrainHeightMap,lod);
            
            return new TerrainData(meshData,terrainHeightMap);
        }
        
        public float[,] GenerateHeightMapForOrigin(Vector2 tile)
        {
            lock (definition.NoiseSettings)
            {
                float[,] cachedNoiseMap = new float[TerrainDefinition.MAP_CHUNK_SIZE, TerrainDefinition.MAP_CHUNK_SIZE];
            
                Vector3 cachedPositionOffset = definition.NoiseSettings.positionOffset;
            
                Vector2 noiseSpaceOffset = new Vector2(tile.x * definition.NoiseSettings.scaleOffset.x, -tile.y * definition.NoiseSettings.scaleOffset.y);

                definition.NoiseSettings.positionOffset = noiseSpaceOffset;
                
                Noise.GenerateNoiseMap(ref cachedNoiseMap, definition.NoiseSettings);

                definition.NoiseSettings.positionOffset = cachedPositionOffset;

                return cachedNoiseMap;
            }
        }
        
        public TerrainMeshData GenerateTerrainMesh(float[,] terrainHeightMap, int lod)
        {
            int width = TerrainDefinition.MAP_CHUNK_SIZE;
            int height = TerrainDefinition.MAP_CHUNK_SIZE;

            float topLeftCornerX = (width - 1) / -2f;
            float topLeftCornerZ = (height - 1) / 2f;

            int meshSimplificationStep = lod == 0 ? 1 : lod * 2;
            int meshResolution = (width - 1) / meshSimplificationStep + 1;

            TerrainMeshData meshData = new TerrainMeshData(width, height);
            int vertexIndex = 0;

            for (int y = 0; y < height; y += meshSimplificationStep)
            {
                for (int x = 0; x < width; x += meshSimplificationStep)
                {
                    lock (definition.HeightCurve)
                    {
                        float currentHeight = definition.HeightCurve.Evaluate(terrainHeightMap[x, y]) * definition.HeightRange;
                        meshData.vertices[vertexIndex] = new Vector3(topLeftCornerX + x, currentHeight, topLeftCornerZ - y) + definition.TerrainOffset;
                    }

                    meshData.uvs[vertexIndex] = new Vector2(x / (float) width, y / (float) height);

                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + meshResolution + 1, vertexIndex + meshResolution);
                        meshData.AddTriangle(vertexIndex + meshResolution + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            meshData.ApplyFlatShading();
            return meshData;
        }

        public Texture2D GenerateTerrainTexture(float[,] terrainHeightMap)
        {
            int width = TerrainDefinition.MAP_CHUNK_SIZE;
            int height = TerrainDefinition.MAP_CHUNK_SIZE;

            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            NativeArray<Color32> textureArray = texture.GetRawTextureData<Color32>();

            TerrainLayer topTerrainLayer = new TerrainLayer()
            {
                height = 1.0f,
                terrainColor = definition.TerrainLayers.Last().terrainColor
            };
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int i = 0; i < definition.TerrainLayers.Length; i++)
                    {
                        if (terrainHeightMap[x, y] >= definition.TerrainLayers[i].height)
                        {
                            TerrainLayer downLayer = definition.TerrainLayers[i];
                            TerrainLayer upLayer = i == definition.TerrainLayers.Length - 1  ? topTerrainLayer : definition.TerrainLayers[i + 1];
                            
                            float layerHeight = upLayer.height - downLayer.height;
                            float positionOnLayer = terrainHeightMap[x, y] - downLayer.height;
                            float currentLayerPositionRatio = positionOnLayer / layerHeight;
                            
                            Color blendedColor = Color.Lerp(downLayer.terrainColor,upLayer.terrainColor, currentLayerPositionRatio);
                            textureArray[y * height + x] = blendedColor;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            texture.Apply();

            return texture;
        }
        
        private void ProceedRequestedTerrains()
        {
            ThreadRequestData<TerrainData> requestedMeshData = requestedTerrainQueue.Dequeue();
            requestedMeshData.callback?.Invoke(requestedMeshData.requestedData);
        }
        
        #region Editor

        #if UNITY_EDITOR

        private void GenerateAndDisplayTerrain()
        {
            if (definition == null)
            {
                return;
            }
            
            TerrainData terrainData = Generate(definition.LevelOfDetails, definition.NoiseSettings.positionOffset);
            Texture2D terrainTexture = GenerateTerrainTexture(terrainData.heightMap);
            terrainRenderer.DisplayMesh(terrainData.meshData, terrainTexture, definition.ChunkSize);
        }

        #endif

        #endregion Editor
    }
}

