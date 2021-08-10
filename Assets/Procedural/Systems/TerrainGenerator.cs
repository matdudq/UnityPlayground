using System;
using System.Collections;
using System.Linq;
using DudeiNoise;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

namespace Procedural
{
    public partial class TerrainGenerator : SingletonMonoBehaviour<TerrainGenerator>
    {
        #region Variables

        [SerializeField]
        private TerrainDefinition definition = null;

        #if UNITY_EDITOR
        [SerializeField, Tooltip("Component which displays preview in edit mode.")]
        private TerrainPreview terrainPreview = null;
        #endif
        
        #endregion Variables

        #region Public methods

        public void RequestTerrainByJob(int lod, Vector2 tile, Action<RequestedTerrainData> onRequest = null)
        {
            GenerateHeightMapForOrigin(tile, lod, OnGenerateHeightMapCompleted);
            
            void OnGenerateHeightMapCompleted(NoiseTexture heightMap)
            {
                GenerateMeshAndTexture(lod, heightMap, onRequest);
            }
        }

        #endregion Public methods

        #region Private methods

        private void GenerateMeshAndTexture(int lod, NoiseTexture noiseTexture, Action<RequestedTerrainData> onRequest = null)
        {
            StartCoroutine(GenerateMeshAndTextureProcess());
            
            IEnumerator GenerateMeshAndTextureProcess()
            {
                NativeArray<Color> noiseMap = noiseTexture.Texture.GetRawTextureData<Color>();

                JobHandle meshJobHandle = GenerateTerrainMesh(lod, 
                                                                 noiseMap,
                                                                 out NativeArray<float3> vertices,
                                                                 out NativeArray<float2> uvs,
                                                                 out NativeArray<int> triangles);

                int textureResolution = GetSimplifiedMeshResolution(TerrainDefinition.MAP_CHUNK_SIZE, lod);

                Texture2D meshTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
                meshTexture.filterMode = FilterMode.Point;
                meshTexture.wrapMode = TextureWrapMode.Clamp;
                
                NativeArray<TerrainLayer> terrainLayers = new NativeArray<TerrainLayer>(definition.TerrainLayers,Allocator.TempJob);

                Coroutine textureJobCourutine = GenerateMeshTexture(meshTexture, noiseMap, terrainLayers, meshJobHandle);
                
                yield return textureJobCourutine;
                
                meshTexture.Apply();

                Mesh terrainMesh = ConstructMesh(vertices, uvs, triangles);

                terrainLayers.Dispose();
                vertices.Dispose();
                uvs.Dispose();
                triangles.Dispose();
                
                onRequest?.Invoke(new RequestedTerrainData(terrainMesh, meshTexture));
            }
        }
        
        private void GenerateHeightMapForOrigin(Vector2 tile, int lod, Action<NoiseTexture> onCompleted)
        {
            NoiseTexture noiseTexture = new NoiseTexture(GetSimplifiedMeshResolution(TerrainDefinition.MAP_CHUNK_SIZE, lod));
            
            Vector2 noiseSpaceOffset = new Vector2(tile.x * definition.NoiseSettings.scaleOffset.x, -tile.y * definition.NoiseSettings.scaleOffset.y);
        
            definition.NoiseSettings.positionOffset = noiseSpaceOffset;
            
            noiseTexture.GenerateNoiseForChanelAsync(definition.NoiseSettings, NoiseTextureChannel.RED, transform, onCompleted);
        }
        
        private Coroutine GenerateMeshTexture(Texture2D meshTexture, NativeArray<Color> noiseMap, NativeArray<TerrainLayer> layers, JobHandle dependsOn)
        {
            TerrainLayer topTerrainLayer = new TerrainLayer()
            {
                height = 1.0f,
                terrainColor = definition.TerrainLayers.Last().terrainColor
            };
            
            NativeArray<Color32> textureArray = meshTexture.GetRawTextureData<Color32>();
            
            int meshSize = textureArray.Length;

            GenerateTextureJob generateTextureJob = new GenerateTextureJob()
            {
                topTerrainLayer = topTerrainLayer,
                noiseMap = noiseMap,
                terrainLayers = layers,
                textureArray = textureArray
            };
            
            return generateTextureJob.ScheduleCoroutine(meshSize, meshSize / 6, this);
        }
        
        private JobHandle GenerateTerrainMesh(int lod, NativeArray<Color>noiseMap, out NativeArray<float3> vertices, out NativeArray<float2> uvs, out NativeArray<int> triangles)
        {
            int fullMeshResolution = TerrainDefinition.MAP_CHUNK_SIZE;
            int meshSize = fullMeshResolution * fullMeshResolution;
            int simplifiedMeshResolution = GetSimplifiedMeshResolution(fullMeshResolution, lod);

            float heightRange = definition.HeightRange;
            float3 meshOffset = definition.TerrainOffset;
            int trianglesCount = simplifiedMeshResolution * simplifiedMeshResolution * 6;
               
            vertices = new NativeArray<float3>(trianglesCount, Allocator.TempJob);
            uvs = new NativeArray<float2>(trianglesCount, Allocator.TempJob);
            triangles = new NativeArray<int>(trianglesCount, Allocator.TempJob);
            
            GenerateMeshJob generateVerticesJob = new GenerateMeshJob()
            {
                fullMeshResolution = fullMeshResolution,
                simplifiedMeshResolution = simplifiedMeshResolution,
                meshOffset = meshOffset,
                heightRange = heightRange,
                noiseMap = noiseMap,
                vertices = vertices,
                uvs = uvs,
                triangles = triangles
            };

            int jobIterations = (simplifiedMeshResolution - 1) * (simplifiedMeshResolution - 1) * 6;

            return generateVerticesJob.ScheduleAsync(jobIterations, meshSize / 6, this);
        }
        
        private Mesh ConstructMesh(NativeArray<float3> vertices, NativeArray<float2> uvs, NativeArray<int> triangles)
        {
            Mesh mesh = new Mesh();

            mesh.SetVertices(vertices);
            mesh.SetIndices(triangles,MeshTopology.Triangles,0);
            mesh.SetUVs(0,uvs);
            
            mesh.RecalculateNormals();
            return mesh;
        }

        private int GetSimplifiedMeshResolution(int fullMeshResolution, int lod)
        {
            int meshSimplificationStep = lod == 0 ? 1 : lod * 2;
            return (fullMeshResolution - 1) / meshSimplificationStep + 1;
        }

        #endregion Private methods
        
        #region Editor

        #if UNITY_EDITOR

        private void GenerateAndDisplayTerrain()
        {
            if (definition == null)
            {
                return;
            }
            
            RequestTerrainByJob(definition.LevelOfDetails, definition.NoiseSettings.positionOffset, OnTerrainGenerated);
            
            void OnTerrainGenerated(RequestedTerrainData terrainData)
            {
                terrainPreview.DisplayMesh(terrainData.mesh, terrainData.texture);
            }
        }

        #endif

        #endregion Editor
    }
}

