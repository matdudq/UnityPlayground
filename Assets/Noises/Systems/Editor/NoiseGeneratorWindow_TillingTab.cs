﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow
	{
		private class TillingTab : INoiseGeneratorModeTab
		{
			private NoiseGeneratorWindow owner;
			
			private float frequencyValue = 0;
			
			private bool isNoiseTypeSectionFolded = false;
			private bool isFrequencySectionFolded = false;
			private bool isLayersSectionFolded = false;
			private bool isCustomPatternsSectionFolded = false;
			
			public TillingTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
				frequencyValue = owner.scaleOffsetSP.vector3Value.x;
			}
			
			public void OnTabEnter()
			{
				owner.tillingEnabledSP.boolValue = true;
				owner.CurrentNoiseSettingsSP.serializedObject.ApplyModifiedProperties();
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
				return GUILayout.Button(owner.tillingTabButtonGC);
			}

			private void DrawCustomPatternsSection()
			{
				isCustomPatternsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isCustomPatternsSectionFolded, owner.customPatternsSectionHeaderGC);

				if (isCustomPatternsSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.woodPatternMultiplierSP);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.turbulenceSP);
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawNoiseTypeSection()
			{
				isNoiseTypeSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isNoiseTypeSectionFolded, owner.noiseTypeSectionHeaderGC);
				
				if (isNoiseTypeSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.dimensionsSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.noiseTypeSP);
					GUILayout.EndHorizontal();

					owner.tillingPeriodSP.intValue = Mathf.RoundToInt(frequencyValue);
					float halfOfFrequency = frequencyValue * 0.5f;
					owner.positionOffsetSP.vector3Value = new Vector3(halfOfFrequency, halfOfFrequency, halfOfFrequency);
					
					GUILayout.Space(10);
					GUILayout.EndVertical();
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawFrequencySection()
			{
				isFrequencySectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isFrequencySectionFolded, owner.frequencySectionHeaderGC);

				if (isFrequencySectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					frequencyValue = Mathf.RoundToInt(EditorGUILayout.FloatField("Frequency", frequencyValue));
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					owner.scaleOffsetSP.vector3Value = frequencyValue * Vector3.one;
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawLayersSettingsSection()
			{
				isLayersSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isLayersSectionFolded, owner.octavesSectionHeaderGC);
				
				if (isLayersSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.octavesSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.lacunaritySP);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.persistenceSP);
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