#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow
	{
		private class TillingTab : INoiseGeneratorWindowTab
		{
			private GUIContent buttonContent = null;
			private GUIContent noiseDataHeaderGC = null;
			
			private GUIContent customPatternsHeaderGC = null;
		
			private SerializedProperty positionOffsetSP = null;
			private SerializedProperty scaleOffsetSP    = null;
			
			private SerializedProperty tillingPeriodSP  = null;
			private SerializedProperty tillingEnabledSP = null;

			private SerializedProperty dimensionsSP            = null;
			private SerializedProperty octavesSP               = null;
			private SerializedProperty lacunaritySP            = null;
			private SerializedProperty persistenceSP           = null;
			private SerializedProperty woodPatternMultiplierSP = null;
		
			private SerializedProperty turbulenceSP = null;
		
			private SerializedProperty noiseTypeSP = null;

			private bool isNoiseTypeSectionFolded = false;
			private bool isSpaceSettingsSectionFolded = false;
			private bool isLayersSettingsSectionFolded = false;
			private bool isCustomPatternsSectionFolded = false;
			
			private NoiseGeneratorWindow owner;
			
			private float frequencyValue = 0;
		
			private bool useAdvancedSpaceSettings = false;
			
			public TillingTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
				
				this.buttonContent = new GUIContent("Tilling Mode");
				noiseDataHeaderGC = new GUIContent("Noise settings");
				customPatternsHeaderGC = new GUIContent("Custom patterns");
				
				UpdateActiveSerializedProperties();
			
				frequencyValue = scaleOffsetSP.vector3Value.x;
			}

			private void UpdateActiveSerializedProperties()
			{
				positionOffsetSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("positionOffset");
				scaleOffsetSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("scaleOffset");
				tillingEnabledSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("tillingEnabled");
				tillingPeriodSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("tillingPeriod");
				dimensionsSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("dimensions");
				octavesSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("octaves");
				lacunaritySP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("lacunarity");
				persistenceSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("persistence");
				woodPatternMultiplierSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("woodPatternMultiplier");
				turbulenceSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("turbulence");
				noiseTypeSP = owner.CurrentNoiseSettingsSP.FindPropertyRelative("noiseType");
			}

			public void OnTabEnter()
			{
				tillingEnabledSP.boolValue = true;
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
				DrawNoiseTypeSection();
				DrawSpaceSettingsSection();
				DrawLayersSettingsSection();
				DrawCustomPatternsSection();
			}

			public bool DrawButton()
			{
				return GUILayout.Button(buttonContent);
			}

			private void DrawCustomPatternsSection()
			{
				isCustomPatternsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isCustomPatternsSectionFolded, customPatternsHeaderGC);

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
				isNoiseTypeSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isNoiseTypeSectionFolded, noiseDataHeaderGC);
				
				if (isNoiseTypeSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(dimensionsSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(noiseTypeSP);
					GUILayout.EndHorizontal();

					tillingPeriodSP.intValue = Mathf.RoundToInt(frequencyValue);
					float halfOfFrequency = frequencyValue * 0.5f;
					positionOffsetSP.vector3Value = new Vector3(halfOfFrequency, halfOfFrequency, halfOfFrequency);
					
					GUILayout.Space(10);
					GUILayout.EndVertical();
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawSpaceSettingsSection()
			{
				isSpaceSettingsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isSpaceSettingsSectionFolded, "Frequency settings");

				if (isSpaceSettingsSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					frequencyValue = Mathf.RoundToInt(EditorGUILayout.FloatField("Frequency", frequencyValue));
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					scaleOffsetSP.vector3Value = frequencyValue * Vector3.one;
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawLayersSettingsSection()
			{
				isLayersSettingsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isLayersSettingsSectionFolded, "Octaves settings");
				
				if (isLayersSettingsSectionFolded)
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