using UnityEngine;

namespace Procedural
{
    public class TerrainMeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        private int traingleIndex;
        
        public TerrainMeshData(int width, int height)
        {
            vertices = new Vector3[width*height];
            uvs = new Vector2[width*height]; 
            triangles = new int[(width-1)*(height-1)*6];

            traingleIndex = 0;
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[traingleIndex] = a;
            triangles[traingleIndex + 1] = b;
            triangles[traingleIndex + 2] = c;

            traingleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs
            };

            mesh.RecalculateNormals();
            return mesh;
        }
        
    }
}