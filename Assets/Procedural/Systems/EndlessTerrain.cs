using System.Collections.Generic;
using UnityEngine;

namespace Procedural
{
    public class EndlessTerrain : MonoBehaviour
    {
        [SerializeField] 
        private LODInfo[] lods = null;
        
        [SerializeField] 
        private Transform observer = null;
        
        [SerializeField]
        private TerrainChunk chunkPrefab = null;

        [SerializeField] 
        private float observerMoveThresholdForLodUpdate = 25.0f;
        
        private Vector2 lastLodUpdateObserverPosition = Vector3.zero;
        
        private int visibleChunksCount = 1;

        private Dictionary<Vector2Int, TerrainChunk> chunkCoordToTerrain = null;
        private List<TerrainChunk> lastUpdateVisibleTerrainChunks = null;
        
        private int ChunkSize
        {
            get
            {
                return TerrainDefinition.MAP_CHUNK_SIZE - 1;
            }
        }

        private Vector2 ObserverPositionXZ
        {
            get
            {
                return new Vector2(observer.position.x,observer.position.z);
            }
        }
        
        private float MaxViewDistance
        {
            get
            {
                return lods[lods.Length - 1].distanceThreshold;
            }
        }
        
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if ((ObserverPositionXZ - lastLodUpdateObserverPosition).magnitude > observerMoveThresholdForLodUpdate)
            {
                lastLodUpdateObserverPosition = ObserverPositionXZ;
                UpdateVisibleChunks();
            }
        }
        
        private void Initialize()
        {
            chunkCoordToTerrain = new Dictionary<Vector2Int, TerrainChunk>();
            lastUpdateVisibleTerrainChunks = new List<TerrainChunk>();
            visibleChunksCount = Mathf.RoundToInt(MaxViewDistance / ChunkSize);
        }

        private void UpdateVisibleChunks()
        {
            foreach (TerrainChunk chunk in lastUpdateVisibleTerrainChunks)
            {
                chunk.SetVisible(false);
            }
            
            lastUpdateVisibleTerrainChunks.Clear();

            Vector2Int currentChunkCoords = new Vector2Int(
                Mathf.RoundToInt(ObserverPositionXZ.x / ChunkSize),
                Mathf.RoundToInt(ObserverPositionXZ.y / ChunkSize));

            for (int y = -visibleChunksCount; y <= visibleChunksCount; y++)
            {
                for (int x = -visibleChunksCount; x <= visibleChunksCount; x++)
                {
                    Vector2Int viewedChunkCoord = currentChunkCoords + new Vector2Int(x,y);

                    if (chunkCoordToTerrain.ContainsKey(viewedChunkCoord))
                    {
                        TerrainChunk viewedChunk = chunkCoordToTerrain[viewedChunkCoord];
                        viewedChunk.UpdateVisibility();
                        if (viewedChunk.IsVisible)
                        {
                            lastUpdateVisibleTerrainChunks.Add(viewedChunk);
                        }
                    }
                    else
                    {
                        TerrainChunk terrainChunk = Instantiate(chunkPrefab);
                        terrainChunk.Initialize(viewedChunkCoord, ChunkSize, transform, lods, observer);
                        chunkCoordToTerrain.Add(viewedChunkCoord, terrainChunk);
                    }
                }
            }
            
            
        }
    }
}