using System;
using System.Collections.Generic;
using System.Threading;
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

        public Texture2D cachedTexture = null;

        private Queue<TerrainMeshDataThreadInfo> requestedTerrainQueue = null;
        
        protected override void Awake()
        {
            base.Awake();
            
            requestedTerrainQueue = new Queue<TerrainMeshDataThreadInfo>();
            
            CheckNoiseMapUpToDate();
            CheckTextureUpToDate();
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
            TerrainMeshDataThreadInfo threadInfo = requestedTerrainQueue.Dequeue();
            threadInfo.callback?.Invoke(threadInfo.terrainMeshData);
        }
        
        private void GenerateAndDisplayTerrain()
        {
            CheckNoiseMapUpToDate();
            CheckTextureUpToDate();
            
            terrainRenderer.DisplayMesh(GenerateTerrainMesh(tempDefinition, tempDefinition.LevelOfDetails), cachedTexture, tempDefinition.ChunkSize);
        }

        public void RequestTerrainMesh(int lod, Action<TerrainMeshData> onRequest)
        {
            Debug.Log(lod);
            ThreadStart threadStart = ThreadProcess;

            new Thread(threadStart).Start();
            
            void ThreadProcess()
            {
                requestedTerrainQueue.Enqueue(new TerrainMeshDataThreadInfo(onRequest, GenerateTerrainMesh(tempDefinition, lod)));
            }
        }
        
        public TerrainMeshData GenerateTerrainMesh(TerrainDefinition terrainDefinition, int lod)
        {
            int width = TerrainDefinition.MAP_CHUNK_SIZE;
            int height = TerrainDefinition.MAP_CHUNK_SIZE;
            
            float topLeftCornerX = (width - 1) / -2f;
            float topLeftCornerZ = (height - 1) / 2f;

            int meshSimplificationStep = lod == 0 ? 1 : lod * 2;
            int meshResolution = (width - 1) / meshSimplificationStep +1;
            
            TerrainMeshData meshData = new TerrainMeshData(width, height);
            int vertexIndex = 0;

            for (int y = 0; y < height; y += meshSimplificationStep)
            {
                for (int x = 0; x < width; x += meshSimplificationStep)
                {
                    lock (terrainDefinition.HeightCurve)
                    {
                        float currentHeight = terrainDefinition.HeightCurve.Evaluate(cachedNoiseMap[x, y]) * terrainDefinition.HeightRange;
                        meshData.vertices[vertexIndex] = new Vector3(topLeftCornerX + x,currentHeight ,topLeftCornerZ - y) + terrainDefinition.TerrainOffset;
                    }
                    
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

        private Texture2D GenerateTerrainTexture(TerrainDefinition terrainDefinition)
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
                    for (int i = 0; i < terrainDefinition.TerrainLayers.Length; i++)
                    {
                        if (cachedNoiseMap[x,y] < terrainDefinition.TerrainLayers[i].height)
                        {
                            textureArray[y * height + x] = terrainDefinition.TerrainLayers[i].terrainColor;
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

            if (cachedNoiseMap == null || width * height != cachedNoiseMap.Length)
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

        private void CheckTextureUpToDate()
        {
            if (cachedTexture == null)
            {
                cachedTexture = GenerateTerrainTexture(tempDefinition);
            }
        }
    }
}

