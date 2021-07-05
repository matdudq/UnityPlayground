using UnityEngine;

namespace Procedural
{
    public partial class TerrainChunk : MonoBehaviour
    {
        private Vector2 position = Vector2.zero;

        private Bounds bounds = new Bounds();
        
        [SerializeField] 
        private MeshRenderer meshRenderer = null;

        [SerializeField] 
        private MeshFilter meshFilter = null;

        private LODInfo[] detailLevels;
        private LODTerrainMesh[] lodMeshes;

        private int previousLOD = -1;

        //TODO: REFACTOR
        private Transform chunkObserver = null;
        
        public bool IsVisible
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        private static TerrainGenerator TerrainGenerator
        {
            get
            {
                return TerrainGenerator.Instance;
            }
        }

        private float MaxViewDistance
        {
            get
            {
                return detailLevels[detailLevels.Length - 1].distanceThreshold;
            }
        }
        
        private Vector2 ObserverPositionXZ
        {
            get
            {
                return new Vector2(chunkObserver.position.x,chunkObserver.position.z);
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

        public void UpdateVisibility()
        {
            float boundsToObserver = BoundsToPositionDistance(ObserverPositionXZ);
            bool isVisible = boundsToObserver <= MaxViewDistance;

            if (isVisible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (boundsToObserver > detailLevels[i].distanceThreshold)
                    {
                        lodIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (previousLOD != lodIndex)
                {
                    LODTerrainMesh lodTerrainMesh = lodMeshes[lodIndex];
                    if (lodTerrainMesh.hasMesh)
                    {
                        meshFilter.mesh = lodTerrainMesh.mesh;
                    }
                    else if(!lodTerrainMesh.hasRequestedMesh)
                    {
                        lodTerrainMesh.RequestTerrainMesh();
                    }
                }
            }
            
            SetVisible(isVisible);
        }

        public void Initialize(Vector2 coord, int size, Transform parent, LODInfo[] detailLevels, Transform chunkObserver)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);

            transform.position = new Vector3(position.x, 0, position.y);
            transform.parent = parent;

            SetVisible(false);
            
            meshRenderer.sharedMaterial.mainTexture = TerrainGenerator.cachedTexture;
            
            this.detailLevels = detailLevels;
            lodMeshes = new LODTerrainMesh[detailLevels.Length];

            for (int i = 0; i < lodMeshes.Length; i++)
            {
                lodMeshes[i] = new LODTerrainMesh(detailLevels[i].lodLevel, UpdateVisibility);
            }

            this.chunkObserver = chunkObserver;

            UpdateVisibility();
        }
    }
}