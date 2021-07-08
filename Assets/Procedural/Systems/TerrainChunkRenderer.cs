using UnityEngine;

namespace Procedural
{
	public class TerrainChunkRenderer : MonoBehaviour
	{
		[SerializeField]
		private MeshRenderer meshRenderer = null;

		[SerializeField]
		private MeshFilter meshFilter = null;
		
		private Bounds bounds = new Bounds();
		
		public void SetVisible(bool visible)
		{
			gameObject.SetActive(visible);
		}

		public void Initialize(Vector2 coord, int size, Transform parent)
		{
			Vector2 position = coord * size;
			bounds = new Bounds(position, Vector2.one * size);
			
			transform.position = new Vector3(position.x, 0, position.y);
			transform.parent = parent;
			SetVisible(false);
		}

		public void SetTexture(Texture2D newTexture)
		{
			meshRenderer.sharedMaterial.mainTexture = newTexture;
		}

		public void SetMesh(Mesh mesh)
		{
			meshFilter.mesh = mesh;
		}
		
		public float BoundsToPositionDistance(Vector3 position)
		{
			return Mathf.Sqrt(bounds.SqrDistance(position));
		}
	}
}