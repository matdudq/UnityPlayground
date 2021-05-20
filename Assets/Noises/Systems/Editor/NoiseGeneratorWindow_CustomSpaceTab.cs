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
			
			private GUIContent noiseTypeSectionHeaderGC = null;
			private GUIContent customPatternsSectionHeaderGC = null;	
			private GUIContent spaceSectionHeaderGC = null;
			private GUIContent octavesSectionHeaderGC = null;
			
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
			private bool isOctavesSectionFolded = false;
			private bool isCustomPatternsSectionFolded = false;
			
			public CustomSpaceTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
				
				buttonContent = new GUIContent("Custom Space Mode");
				
				noiseTypeSectionHeaderGC = new GUIContent("Noise Type");
				customPatternsSectionHeaderGC = new GUIContent("Custom patterns");
				spaceSectionHeaderGC = new GUIContent("Space settings");
				octavesSectionHeaderGC = new GUIContent("Octaves settings");
				
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
				
				DrawNoiseTypeSection();
				DrawSpaceSettingsSection();
				DrawOctavesSettings();
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
				isCustomPatternsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isCustomPatternsSectionFolded, customPatternsSectionHeaderGC);

				if (isCustomPatternsSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(woodPatternMultiplierSP);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(turbulenceSP);
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}	
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawNoiseTypeSection()
			{
				isNoiseTypeSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isNoiseTypeSectionFolded, noiseTypeSectionHeaderGC);
				
				if (isNoiseTypeSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(noiseTypeSP);				
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(dimensionsSP);
					GUILayout.EndHorizontal();


					GUILayout.Space(10);
					GUILayout.EndVertical();
				}
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawSpaceSettingsSection()
			{
				isOctavesSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isOctavesSectionFolded, spaceSectionHeaderGC);

				if (isOctavesSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(positionOffsetSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(rotationOffsetSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(scaleOffsetSP);
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawOctavesSettings()
			{
				isOctavesSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isOctavesSectionFolded, octavesSectionHeaderGC);
				
				if (isOctavesSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(octavesSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(lacunaritySP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(persistenceSP);
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
			}
		}
	}
}
#endif