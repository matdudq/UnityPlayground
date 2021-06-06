using UnityEngine;

namespace Procedural
{
    public class TerrainRenderer : MonoBehaviour
    {
        [SerializeField] 
        private MeshFilter terrainFilter = null;
        
        [SerializeField] 
        private MeshRenderer terrainRenderer = null;
        
        public void DisplayMesh(TerrainMeshData meshData, Texture2D texture, float size)
        {
            terrainFilter.sharedMesh = meshData.CreateMesh();
            terrainRenderer.sharedMaterial.mainTexture = texture;

            transform.localScale = Vector3.one * size;
        }
    }
}