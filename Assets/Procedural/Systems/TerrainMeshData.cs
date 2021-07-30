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

        public void ApplyFlatShading()
        {
            Vector3[] flatShadedVertices = new Vector3[triangles.Length];
            Vector2[] flatShadedUV = new Vector2[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                flatShadedVertices[i] = vertices[triangles[i]];
                flatShadedUV[i] = uvs[triangles[i]];
                triangles[i] = i;
            }

            vertices = flatShadedVertices;
            uvs = flatShadedUV;
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