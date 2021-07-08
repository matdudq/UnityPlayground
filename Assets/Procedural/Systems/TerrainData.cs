using UnityEngine;

namespace Procedural
{
	public class TerrainData
	{
		public TerrainMeshData meshData = null;
		public Texture2D terrainTexture = null;

		public TerrainData(TerrainMeshData meshData, Texture2D terrainTexture)
		{
			this.meshData = meshData;
			this.terrainTexture = terrainTexture;
		}
	}
}