using DudeiNoise;
using UnityEngine;

namespace Procedural
{
	public class TerrainData
	{
		public TerrainMeshData meshData = null;
		public NoiseTexture heightMap = null;
		public Texture2D texture = null;
		
		public TerrainData(TerrainMeshData meshData, NoiseTexture heightMap, Texture2D texture)
		{
			this.meshData = meshData;
			this.heightMap = heightMap;
			this.texture = texture;
		}
	}
}