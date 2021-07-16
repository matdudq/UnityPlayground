using UnityEngine;

namespace Procedural
{
	public static class TerrainDataExtension
	{
		public static Texture2D GenerateTerrainTexture(this TerrainData terrainData)
		{
			return TerrainGenerator.Instance.GenerateTerrainTexture(terrainData.heightMap);
		}

		public static Mesh GenerateTerrainMesh(this TerrainData terrainData)
		{
			return terrainData.meshData.CreateMesh();
		}
	}
}