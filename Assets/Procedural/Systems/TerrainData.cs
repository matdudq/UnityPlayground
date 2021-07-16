using UnityEngine;

namespace Procedural
{
	public class TerrainData
	{
		public TerrainMeshData meshData = null;
		public float[,] heightMap = null;

		public TerrainData(TerrainMeshData meshData, float[,] heightMap)
		{
			this.meshData = meshData;
			this.heightMap = heightMap;
		}
	}
}