using DudeiNoise;
using DudeiNoise.Editor;
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
            
            private SerializedProperty terrainPreviewDefinitionSP = null;

            private Editor terrainPreviewDefinitionEditor  = null;

            private bool autoUpdate = true;

            public Editor TerrainPreviewDefinitionEditor
            {
                get
                {
                    if (terrainGenerator != null && terrainPreviewDefinitionEditor == null)
                    {
                        terrainPreviewDefinitionEditor = CreateEditor(terrainGenerator.definition);
                    }
                    
                    return terrainPreviewDefinitionEditor;
                }
            }

            private void OnEnable()
            {
                terrainGenerator = (target as TerrainGenerator);
                buttonTitleGC = new GUIContent("Regenerate");
                autoUpdateToogleGC = new GUIContent("Auto update");
                terrainPreviewDefinitionSP = serializedObject.FindProperty("definition");
            }

            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();
                
                DrawDefaultInspector();

                if (terrainPreviewDefinitionSP.objectReferenceValue != null)
                {
                    GUILayout.BeginVertical();
                
                    TerrainPreviewDefinitionEditor.DrawDefaultInspector();
                    
                    GUILayout.EndVertical();
                }
                
                autoUpdate = GUILayout.Toggle(autoUpdate, autoUpdateToogleGC);
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (autoUpdate)
                    {
                        terrainGenerator.GenerateAndDisplayTerrain();
                    }
                }

                if (!autoUpdate && GUILayout.Button(buttonTitleGC))
                {
                    terrainGenerator.GenerateAndDisplayTerrain();
                }
            }
        }
    }
}