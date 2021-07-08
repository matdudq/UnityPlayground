using System;
using UnityEngine;

namespace Procedural
{
    public partial class EndlessTerrain
    {
        private class LODTerrainMesh
        {
            public Texture2D texture2D = null;
            public int lod = 0;
            public Mesh mesh = null;
            public bool hasRequestedMesh = false;
            public bool hasMesh = false;

            private Action meshReceivedCallback = null;
            
            public LODTerrainMesh(int lod, Action meshReceivedCallback = null)
            {
                this.lod = lod;
                this.meshReceivedCallback = meshReceivedCallback;
            }

            public void RequestTerrainMesh()
            {
                hasRequestedMesh = true;
                TerrainGenerator.Instance.RequestTerrain(lod,Vector2.zero, OnTerrainMeshReceived);
            }
            
            private void OnTerrainMeshReceived(TerrainData terrainMeshData)
            {
                hasMesh = true;
                mesh = terrainMeshData.meshData.CreateMesh();
                texture2D = terrainMeshData.terrainTexture;
                meshReceivedCallback?.Invoke();
            }
        }
    }
    

}