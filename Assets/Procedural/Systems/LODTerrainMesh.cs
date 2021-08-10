using System;
using UnityEngine;

namespace Procedural
{
    public partial class EndlessTerrain
    {
        private class LODTerrainMesh
        {
            public int lod = 0;
            public Vector2 tilePosition = Vector2.zero;
            
            public Mesh mesh = null;
            public Texture2D texture2D = null;
            
            public bool hasRequestedMesh = false;
            public bool hasMesh = false;

            private event Action meshReceivedCallback = null;
            
            public LODTerrainMesh(int lod, Vector2 tilePosition, Action meshReceivedCallback = null)
            {
                this.lod = lod;
                this.meshReceivedCallback = meshReceivedCallback;
                this.tilePosition = tilePosition;
            }

            public void RequestTerrainMesh()
            {
                hasRequestedMesh = true;
                TerrainGenerator.Instance.RequestTerrainByJob(lod, tilePosition, OnTerrainMeshReceived);
            }
            
            private void OnTerrainMeshReceived(TerrainData terrainMeshData)
            {
                hasMesh = true;
                mesh = terrainMeshData.mesh;
                texture2D = terrainMeshData.texture;
                meshReceivedCallback?.Invoke();
            }
        }
    }
    

}