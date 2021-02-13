using UnityEngine;

public class TangentSpaceVisualizer : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter)
		{
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh)
			{
				for (int i = 0; i < mesh.vertexCount; i++)
				{
					ShowTangentSpace(transform.TransformPoint(mesh.vertices[i]),
									 transform.TransformDirection(mesh.normals[i]),
									 transform.TransformDirection(mesh.tangents[i]),
									 mesh.tangents[i].w);
				}
			}
		}
	}

	public float scale = 0.1f;
	public float vertexOffset = 0.01f;
	
	private void ShowTangentSpace(Vector3 vertex, Vector3 normal, Vector3 tangent, float bitangentSign)
	{
		vertex += normal * vertexOffset;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(vertex, vertex + normal*scale);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(vertex, vertex+tangent*scale);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(vertex,vertex + Vector3.Cross(normal,tangent) * bitangentSign * scale);
	}
}
