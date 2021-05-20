#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow
	{
		private class CustomSpaceTab : INoiseGeneratorWindowTab
		{
			private GUIContent buttonContent = null;
			private GUIContent noiseDataHeaderGC = null;
			
			private GUIContent customPatternsHeaderGC = null;
			
			private SerializedProperty positionOffsetSP = null;
			private SerializedProperty rotationOffsetSP = null;
			private SerializedProperty scaleOffsetSP    = null;
			
			private SerializedProperty tillingEnabledSP = null;

			private SerializedProperty dimensionsSP            = null;
			private SerializedProperty octavesSP               = null;
			private SerializedProperty lacunaritySP            = null;
			private SerializedProperty persistenceSP           = null;
			private SerializedProperty woodPatternMultiplierSP = null;
		
			private SerializedProperty turbulenceSP = null;
		
			private SerializedProperty noiseTypeSP = null;
			
			private NoiseGeneratorWindow owner;

			private GUIStyle headerStyle = null;

			private bool useAdvancedSpaceSettings = false;

			private bool isNoiseTypeSectionFolded = false;
			private bool isSpaceSectionFolded = false;
			private bool isLayersSectionFolded = false;
			private bool isCustomPatternsSectionFolded = false;
			
			public CustomSpaceTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
				
				this.buttonContent = new GUIContent("Custom Space Mode");
				noiseDataHeaderGC = new GUIContent("Noise settings");
				customPatternsHeaderGC = new GUIContent("Custom patterns");

				UpdateActiveSerializedProperties();
			}

			public void OnTabEnter()
			{
				tillingEnabledSP.boolValue = false;
				owner.CurrentNoiseSettingsSP.serializedObject.ApplyModifiedProperties();

				UpdateActiveSerializedProperties();
				owner.RegenerateTextures();
			}

			public void OnTabExit()
			{
			}

			public void OnChannelChange()
			{
				UpdateActiveSerializedProperties();
				owner.RegenerateTextures();
			}

			public void DrawInspector()
			{
				if (headerStyle == null)
				{
					headerStyle = new GUIStyle(GUI.skin.label)
					{
						fontStyle = FontStyle.Bold
					};
				}
				
				DrawNoiseTypeTab();
				DrawSpaceSettings();
				DrawLayersSettings();
				DrawCustomPatternsTab();
			}

			public bool DrawButton()
			{
				return GUILayout.Button(buttonContent);
			}

			private void UpdateActiveSerializedProperties()
			{
				positionOffsetSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("positionOffset");
				rotationOffsetSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("rotationOffset");
				scaleOffsetSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("scaleOffset");
				tillingEnabledSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("tillingEnabled");
				dimensionsSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("dimensions");
				octavesSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("octaves");
				lacunaritySP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("lacunarity");
				persistenceSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("persistence");
				woodPatternMultiplierSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("woodPatternMultiplier");
				turbulenceSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("turbulence");
				noiseTypeSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("noiseType");
			}
			
			private void DrawCustomPatternsTab()
			{
				GUILayout.Label(customPatternsHeaderGC,headerStyle);
	            
				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(woodPatternMultiplierSP);
				EditorGUILayout.PropertyField(turbulenceSP);
			}

			private void DrawNoiseTypeTab()
			{
				GUILayout.Label(noiseDataHeaderGC,headerStyle);
				
				EditorGUILayout.PropertyField(noiseTypeSP);
				EditorGUILayout.PropertyField(dimensionsSP);

				EditorGUILayout.Space();
			}

			private void DrawSpaceSettings()
			{
				EditorGUILayout.PropertyField(positionOffsetSP);
				EditorGUILayout.PropertyField(rotationOffsetSP);
				EditorGUILayout.PropertyField(scaleOffsetSP);

				EditorGUILayout.Space();
			}

			private void DrawLayersSettings()
			{
				EditorGUILayout.PropertyField(octavesSP);
				EditorGUILayout.PropertyField(lacunaritySP);
				EditorGUILayout.PropertyField(persistenceSP);
	            
				EditorGUILayout.Space();
			}
		}
	}
}
#endif