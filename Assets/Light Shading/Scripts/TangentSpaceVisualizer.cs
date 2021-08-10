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
				for (int i = firstIndex; i < firstIndex + 50; i++)
				{
					ShowTangentSpace(transform.TransformPoint(mesh.vertices[i]), transform.TransformDirection(mesh.normals[i]),i);
				}
				
				for (int i = secondIndex; i < secondIndex + 50; i++)
				{
					ShowTangentSpace(transform.TransformPoint(mesh.vertices[i]), transform.TransformDirection(mesh.normals[i]),i);
				}
			}
		}
	}

	public float scale = 0.1f;
	public float vertexOffset = 0.01f;
	public int firstIndex = 0;
	public int secondIndex = 0;
	private void ShowTangentSpace(Vector3 vertex, Vector3 normal, int index)
	{
		int moduloIndex = index % 6;
		
		vertex += normal * vertexOffset;

		switch (moduloIndex)
		{
			case 0:
			Gizmos.color = Color.green;
			break;
			case 1:
				Gizmos.color = Color.red;
				break;
			case 2:
				Gizmos.color = Color.black;
				break;
			case 3:
				Gizmos.color = Color.blue;
				break;
			case 4:
				Gizmos.color = Color.yellow;
				break;
			case 5:
				Gizmos.color = Color.magenta;
				break;
		}
		
		Gizmos.DrawLine(vertex, vertex + normal*scale);
	}
}
