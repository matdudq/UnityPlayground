using UnityEditor;
using UnityEngine;

namespace Procedural
{
    public partial class TerrainGenerator
    {
        [CustomEditor(typeof(TerrainGenerator))]
        private class TerrainGeneratorEditor : Editor
        {
            private GUIContent buttonTitleGC = null;

            private void OnEnable()
            {
                buttonTitleGC = new GUIContent("Regenerate");
            }

            public override void OnInspectorGUI()
            {
                TerrainGenerator generator = (TerrainGenerator)target;
                
                DrawDefaultInspector();

                if (GUILayout.Button(buttonTitleGC))
                {
                    generator.UpdateNoiseMap();
                    generator.GenerateAndDisplayTerrain();
                }
            }
        }
    }
}