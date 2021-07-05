using UnityEngine;

namespace Procedural
{
    public class TerrainChunk : MonoBehaviour
    {
        private Vector2 position = Vector2.zero;

        private Bounds bounds = new Bounds();
        
        [SerializeField] 
        private MeshRenderer meshRenderer = null;

        [SerializeField] 
        private MeshFilter meshFilter = null;

        public bool IsVisible
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        private int ChunkSize
        {
            get
            {
                return TerrainDefinition.MAP_CHUNK_SIZE - 1;
            }
        }

        private static TerrainGenerator TerrainGenerator
        {
            get
            {
                return TerrainGenerator.Instance;
            }
        }

        private float BoundsToPositionDistance(Vector3 position)
        {
            return Mathf.Sqrt(bounds.SqrDistance(position));
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void UpdateVisibility(Vector2 observerPosition)
        {
            SetVisible(BoundsToPositionDistance(observerPosition) <= ChunkSize);
        }

        public void Initialize(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);

            transform.position = new Vector3(position.x, 0, position.y);
            transform.parent = parent;

            SetVisible(false);
            
            meshRenderer.sharedMaterial.mainTexture = TerrainGenerator.cachedTexture;

            TerrainGenerator.RequestTerrainMesh(OnTerrainMeshReceived);
        }

        private void OnTerrainMeshReceived(TerrainMeshData terrainMeshData)
        {
            meshFilter.mesh = terrainMeshData.CreateMesh();
        }
    }
}