using UnityEditor;
using UnityEngine;

namespace Procedural
{
    public partial class TerrainGenerator
    {
        [CustomEditor(typeof(TerrainGenerator))]
        private class TerrainGeneratorEditor : Editor
        {
            private TerrainGenerator terrainGenerator = null;
            
            private GUIContent buttonTitleGC = null;
            private GUIContent autoUpdateToogleGC = null;
            
            private SerializedProperty targetDefinitionSP = null;

            private Editor targetDefinitionEditor = null;
            private Editor target2DefinitionEditor = null;
            
            private bool autoUpdate = true;

            private void OnEnable()
            {
                terrainGenerator = (target as TerrainGenerator);
                
                buttonTitleGC = new GUIContent("Regenerate");
                autoUpdateToogleGC = new GUIContent("Auto update");
                
                targetDefinitionSP = serializedObject.FindProperty("tempDefinition");
                
                if (terrainGenerator != null)
                {
                    targetDefinitionEditor = CreateEditor(terrainGenerator.tempDefinition);
                    target2DefinitionEditor = CreateEditor(terrainGenerator.tempDefinition.TextureSettings);
                }

            }

            public override void OnInspectorGUI()
            {
                TerrainGenerator generator = (TerrainGenerator)target;

                EditorGUI.BeginChangeCheck();
                
                if (targetDefinitionSP.objectReferenceValue != null)
                {
                    targetDefinitionEditor.DrawDefaultInspector();
                    target2DefinitionEditor.DrawDefaultInspector();
                }
                
                DrawDefaultInspector();

                autoUpdate = GUILayout.Toggle(autoUpdate, autoUpdateToogleGC);
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (autoUpdate)
                    {
                        generator.GenerateAndDisplayTerrain();
                    }
                }

                if (!autoUpdate && GUILayout.Button(buttonTitleGC))
                {
                    generator.GenerateAndDisplayTerrain();
                }
            }
        }
    }
}