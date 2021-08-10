using UnityEngine;

namespace Procedural
{
	public class RequestedTerrainData
	{
		#region Variables

		public readonly Mesh mesh = null;
		public readonly Texture2D texture = null;

		#endregion Variables

		#region Constructor

		public RequestedTerrainData(Mesh mesh, Texture2D texture)
		{
			this.mesh = mesh;
			this.texture = texture;
		}

		#endregion Constructor
	}
}