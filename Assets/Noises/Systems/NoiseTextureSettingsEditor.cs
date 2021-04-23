#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise
{
		[CustomEditor(typeof(NoiseTextureSettings))]
    	public class NoiseTextureSettingsEditor : Editor
    	{
			#region Variables

			private GUIContent textureDataHeaderGC = null;
			private GUIContent noiseDataHeaderGC = null;
			
			private GUIContent useAdvancedToggleGC = null;
			private GUIContent customPatternsHeaderGC   = null;
			
			private GUIStyle headerStyle = null;

			private SerializedProperty exportFolderSP = null;
			private SerializedProperty resolutionSP = null;
			private SerializedProperty colorGradientSP = null;
			private SerializedProperty filterModeSP = null;
			
			private SerializedProperty positionOffsetSP = null;
			private SerializedProperty rotationOffsetSP = null;
			private SerializedProperty scaleOffsetSP = null;

			private SerializedProperty dimensionsSP = null;
			private SerializedProperty octavesSP = null;
			private SerializedProperty lacunaritySP = null;
			private SerializedProperty persistenceSP = null;
			private SerializedProperty woodPatternMultiplierSP = null;
		
			private SerializedProperty turbulenceSP = null;
		
			private SerializedProperty noiseTypeSP = null;

			private bool useAdvancedSpaceSettings = false;

			private float frequencyValue = 0;
			
			#endregion Variables
			
    		#region Unity methods

			public void OnEnable()
			{
				textureDataHeaderGC = new GUIContent("Texture settings");
				noiseDataHeaderGC = new GUIContent("Noise settings");
				
				useAdvancedToggleGC = new GUIContent("Custom space settings");
				customPatternsHeaderGC = new GUIContent("Custom patterns");
					
				exportFolderSP = serializedObject.FindProperty("exportFolder");
				resolutionSP = serializedObject.FindProperty("resolution");
				colorGradientSP = serializedObject.FindProperty("colorGradient");
				filterModeSP = serializedObject.FindProperty("filterMode");
				
				SerializedProperty settingsSP = serializedObject.FindProperty("noiseSettings");

				positionOffsetSP = settingsSP.FindPropertyRelative("positionOffset");
				rotationOffsetSP = settingsSP.FindPropertyRelative("rotationOffset");
				scaleOffsetSP = settingsSP.FindPropertyRelative("scaleOffset");
				dimensionsSP = settingsSP.FindPropertyRelative("dimensions");
				octavesSP = settingsSP.FindPropertyRelative("octaves");
				lacunaritySP = settingsSP.FindPropertyRelative("lacunarity");
				persistenceSP = settingsSP.FindPropertyRelative("persistence");
				woodPatternMultiplierSP = settingsSP.FindPropertyRelative("woodPatternMultiplier");
				turbulenceSP = settingsSP.FindPropertyRelative("turbulence");
				noiseTypeSP = settingsSP.FindPropertyRelative("noiseType");

				frequencyValue = scaleOffsetSP.vector3Value.x;
			}

			public override void OnInspectorGUI()
    		{
    			if (GUILayout.Button("Open Generator"))
    			{
    				NoiseGeneratorWindow.Open(target as NoiseTextureSettings);
    			}
    		}

			public void DrawCustomInspector()
			{
				if (headerStyle == null)
				{
					headerStyle = new GUIStyle(GUI.skin.label)
					{
						fontStyle = FontStyle.Bold
					};
				}
				
				serializedObject.Update();
				
				GUILayout.Label(textureDataHeaderGC,headerStyle);
				
				EditorGUILayout.Space();
				
				EditorGUI.BeginChangeCheck();
				
				EditorGUILayout.PropertyField(exportFolderSP);
				EditorGUILayout.PropertyField(resolutionSP);
				EditorGUILayout.PropertyField(colorGradientSP);
				EditorGUILayout.PropertyField(filterModeSP);
				
				EditorGUILayout.Space();
				
				GUILayout.Label(noiseDataHeaderGC,headerStyle);
				
				EditorGUILayout.Space();
				
				EditorGUILayout.PropertyField(noiseTypeSP);
				
				EditorGUILayout.Space();

				useAdvancedSpaceSettings = EditorGUILayout.Toggle(useAdvancedToggleGC, useAdvancedSpaceSettings);
				if (useAdvancedSpaceSettings)
				{
					EditorGUILayout.PropertyField(positionOffsetSP);
					EditorGUILayout.PropertyField(rotationOffsetSP);
					EditorGUILayout.PropertyField(scaleOffsetSP);
					frequencyValue = scaleOffsetSP.vector3Value.x;
				}
				else
				{
					scaleOffsetSP.vector3Value = Vector3.back;
					frequencyValue = EditorGUILayout.FloatField("Frequency", frequencyValue);
					scaleOffsetSP.vector3Value = frequencyValue * Vector3.one;
				}

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(dimensionsSP);
				
				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(octavesSP);
				EditorGUILayout.PropertyField(lacunaritySP);
				EditorGUILayout.PropertyField(persistenceSP);
				
				EditorGUILayout.Space();
				
				GUILayout.Label(customPatternsHeaderGC,headerStyle);
				
				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(woodPatternMultiplierSP);
				EditorGUILayout.PropertyField(turbulenceSP);

				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty(target);
				}
			}
			
    		#endregion Unity methods
    	}
		#endif
}