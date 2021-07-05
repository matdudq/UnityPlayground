using System;
using UnityEngine;

namespace Procedural
{
    public partial class TerrainChunk : MonoBehaviour
    {
        private class LODTerrainMesh
        {
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
                TerrainGenerator.Instance.RequestTerrainMesh(lod, OnTerrainMeshReceived);
            }
            
            private void OnTerrainMeshReceived(TerrainMeshData terrainMeshData)
            {
                hasMesh = true;
                mesh = terrainMeshData.CreateMesh();
                meshReceivedCallback?.Invoke();
            }
        }
    }
    

}