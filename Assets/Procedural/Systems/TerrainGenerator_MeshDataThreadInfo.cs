using System;

namespace Procedural
{
	public partial class TerrainGenerator
	{
		private struct TerrainMeshDataThreadInfo
		{
			public readonly Action<TerrainMeshData> callback;

			public readonly TerrainMeshData terrainMeshData;

			public TerrainMeshDataThreadInfo(Action<TerrainMeshData> callback, TerrainMeshData terrainMeshData)
			{
				this.callback = callback;
				this.terrainMeshData = terrainMeshData;
			}
		}
	}
}