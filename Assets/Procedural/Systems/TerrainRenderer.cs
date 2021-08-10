using System;
using UnityEngine;

namespace Procedural
{
    public class TerrainRenderer : MonoBehaviour
    {
        [SerializeField] 
        private MeshFilter terrainFilter = null;
        
        [SerializeField] 
        private MeshRenderer terrainRenderer = null;
        
        public void DisplayMesh(Mesh meshData, Texture2D texture, float size)
        {
            terrainFilter.sharedMesh = meshData;
            terrainRenderer.sharedMaterial.mainTexture = texture;

            transform.localScale = Vector3.one * size;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}