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

            private SerializedProperty targetDefinitionSP = null;

            private Editor targetDefinitionEditor = null;
            
            private void OnEnable()
            {
                buttonTitleGC = new GUIContent("Regenerate");

                targetDefinitionSP = serializedObject.FindProperty("tempDefinition");
                targetDefinitionEditor = CreateEditor((target as TerrainGenerator)?.tempDefinition);
            }

            public override void OnInspectorGUI()
            {
                TerrainGenerator generator = (TerrainGenerator)target;

                if (targetDefinitionSP.objectReferenceValue != null)
                {
                    targetDefinitionEditor.DrawDefaultInspector();
                }
                
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