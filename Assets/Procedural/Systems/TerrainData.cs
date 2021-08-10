using DudeiNoise;
using UnityEngine;

namespace Procedural
{
	public class TerrainData
	{
		public Mesh mesh = null;
		public NoiseTexture heightMap = null;
		public Texture2D texture = null;
		
		public TerrainData(Mesh mesh, NoiseTexture heightMap, Texture2D texture)
		{
			this.mesh = mesh;
			this.heightMap = heightMap;
			this.texture = texture;
		}
	}
}