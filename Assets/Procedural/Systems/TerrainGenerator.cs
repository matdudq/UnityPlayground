using System;
using System.Collections.Generic;
using System.Threading;
using DudeiNoise;
using Procedural.Utilities;
using Unity.Collections;
using UnityEngine;
using Utilities;

namespace Procedural
{
    public partial class TerrainGenerator : SingletonMonoBehaviour<TerrainGenerator>
    {
        [SerializeField, Tooltip("DEBUG")]
        private TerrainDefinition tempDefinition = null;

        [SerializeField, Tooltip("DEBUG")]
        private TerrainRenderer terrainRenderer = null;
        
        private Queue<ThreadRequestData<TerrainData>> requestedTerrainQueue   = null;
        
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

        private void ProceedRequestedTerrains()
        {
            ThreadRequestData<TerrainData> requestedMeshData = requestedTerrainQueue.Dequeue();
            requestedMeshData.callback?.Invoke(requestedMeshData.requestedData);
        }
        
        public void RequestTerrain(int lod, Vector2 origin, Action<TerrainData> onRequest = null)
        {
            ThreadStart threadStart = ThreadProcess;

            new Thread(threadStart).Start();

            void ThreadProcess()
            {
                TerrainData terrainData =  Generate(lod, origin);
                
                requestedTerrainQueue.Enqueue(new ThreadRequestData<TerrainData>(onRequest, terrainData));
            }
        }

        public TerrainData Generate(int lod, Vector2 origin)
        {
            float[,] terrainHeightMap = GenerateHeightMapForOrigin(origin);
            TerrainMeshData meshData = GenerateTerrainMesh(terrainHeightMap,lod);
            Texture2D texture2D = GenerateTerrainTexture(terrainHeightMap);
            
            return new TerrainData(meshData,texture2D);
        }
        
        private float[,] GenerateHeightMapForOrigin(Vector2 origin)
        {
            float[,] cachedNoiseMap = new float[TerrainDefinition.MAP_CHUNK_SIZE, TerrainDefinition.MAP_CHUNK_SIZE];
            
            Noise.GenerateNoiseMap(ref cachedNoiseMap, tempDefinition.NoiseSettings);

            return cachedNoiseMap;
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
                    lock (tempDefinition.HeightCurve)
                    {
                        float currentHeight = tempDefinition.HeightCurve.Evaluate(terrainHeightMap[x, y]) * tempDefinition.HeightRange;
                        meshData.vertices[vertexIndex] = new Vector3(topLeftCornerX + x, currentHeight, topLeftCornerZ - y) + tempDefinition.TerrainOffset;
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

            return meshData;
        }

        private Texture2D GenerateTerrainTexture(float[,] terrainHeightMap)
        {
            int width = TerrainDefinition.MAP_CHUNK_SIZE;
            int height = TerrainDefinition.MAP_CHUNK_SIZE;

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
                        if (terrainHeightMap[x, y] >= tempDefinition.TerrainLayers[i].height)
                        {
                            textureArray[y * height + x] = tempDefinition.TerrainLayers[i].terrainColor;
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
        
        #region Editor

        #if UNITY_EDITOR

        private void GenerateAndDisplayTerrain()
        {
            TerrainData terrainData = Generate(tempDefinition.LevelOfDetails, Vector2.zero);
            terrainRenderer.DisplayMesh(terrainData.meshData, terrainData.terrainTexture, tempDefinition.ChunkSize);
        }

        #endif

        #endregion Editor
    }
}

