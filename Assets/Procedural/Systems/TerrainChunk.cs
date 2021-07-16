using UnityEngine;

namespace Procedural
{
	public partial class EndlessTerrain
	{
		private class TerrainChunk
		{
			private EndlessTerrain chunkOwner = null;

			private TerrainChunkRenderer chunkRenderer = null;
			
			private Vector2 position = Vector2.zero;
			
			private LODTerrainMesh[] lodMeshes;

			private int previousLOD = -1;
			
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
					return chunkOwner.MaxViewDistance;
				}
			}

			private Vector2 ObserverPositionXZ
			{
				get
				{
					return chunkOwner.ObserverPositionXZ;
				}
			}

			private LODInfo[] LODs
			{
				get
				{
					return chunkOwner.lods;
				}
			}
			
			public TerrainChunk(TerrainChunkRenderer chunkRenderer, EndlessTerrain chunkOwner)
			{
				this.chunkRenderer = chunkRenderer;

				this.chunkOwner = chunkOwner;
				
				lodMeshes = new LODTerrainMesh[LODs.Length];

				for (int i = 0; i < lodMeshes.Length; i++)
				{
					lodMeshes[i] = new LODTerrainMesh(LODs[i].lodLevel, chunkRenderer.Coords, UpdateVisibility);
				}
				
				UpdateVisibility();
			}
			
			public void UpdateVisibility()
			{
				float boundsToObserver = chunkRenderer.BoundsToPositionDistance(ObserverPositionXZ);
				bool isVisible = boundsToObserver <= MaxViewDistance;

				if (isVisible)
				{
					int lodIndex = 0;
					for (int i = 0; i < LODs.Length - 1; i++)
					{
						if (boundsToObserver > LODs[i].distanceThreshold)
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
							chunkRenderer.SetMesh(lodTerrainMesh.mesh);
							chunkRenderer.SetTexture(lodTerrainMesh.texture2D);
						}
						else if (!lodTerrainMesh.hasRequestedMesh)
						{
							lodTerrainMesh.RequestTerrainMesh();
						}
					}
					
					chunkOwner.lastUpdateVisibleTerrainChunks.Add(this);
				}

				chunkRenderer.SetVisible(isVisible);
			}

			public void ForcedSetEnabled(bool enabled)
			{
				chunkRenderer.SetVisible(enabled);
			}
		}
	}
}