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
			
			private GUIContent noiseTypeSectionHeaderGC = null;
			private GUIContent customPatternsSectionHeaderGC = null;
			private GUIContent frequencySectionHeaderGC = null;
			private GUIContent octavesSectionHeaderGC = null;
			
			
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

			private NoiseGeneratorWindow owner;
			
			private float frequencyValue = 0;
		
			private bool useAdvancedSpaceSettings = false;
			
			private bool isNoiseTypeSectionFolded = false;
			private bool isFrequencySectionFolded = false;
			private bool isLayersSectionFolded = false;
			private bool isCustomPatternsSectionFolded = false;
			
			public TillingTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
				
				buttonContent = new GUIContent("Tilling Mode");
				
				noiseTypeSectionHeaderGC = new GUIContent("Noise Type");
				customPatternsSectionHeaderGC = new GUIContent("Custom patterns");
				frequencySectionHeaderGC = new GUIContent("Frequency settings");
				octavesSectionHeaderGC = new GUIContent("Octaves settings");
				
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
				DrawFrequencySection();
				DrawLayersSettingsSection();
				DrawCustomPatternsSection();
			}

			public bool DrawButton()
			{
				return GUILayout.Button(buttonContent);
			}

			private void DrawCustomPatternsSection()
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

			private void DrawFrequencySection()
			{
				isFrequencySectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isFrequencySectionFolded, frequencySectionHeaderGC);

				if (isFrequencySectionFolded)
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
				isLayersSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isLayersSectionFolded, octavesSectionHeaderGC);
				
				if (isLayersSectionFolded)
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