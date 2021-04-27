using UnityEditor;
using UnityEngine;

namespace DudeiNoise
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
			
			private NoiseGeneratorWindow owner;

			private GUIStyle headerStyle = null;
			
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
			}

			public void OnTabExit()
			{
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
				
				EditorGUI.BeginChangeCheck();

				DrawNoiseTypeTab();
				DrawSpaceSettings();
				DrawLayersSettings();
				DrawCustomPatternsTab();
				
				if (EditorGUI.EndChangeCheck())
				{
					owner.RegenerateTextures();
					positionOffsetSP.serializedObject.ApplyModifiedProperties();	
					EditorUtility.SetDirty(owner.settings);
				}
			}

			public bool DrawButton()
			{
				return GUILayout.Button(buttonContent);
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
            				
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(dimensionsSP);
				EditorGUILayout.PropertyField(noiseTypeSP);
				
				tillingPeriodSP.intValue = Mathf.RoundToInt(frequencyValue);
				float halfOfFrequency = frequencyValue * 0.5f;
				positionOffsetSP.vector3Value = new Vector3(halfOfFrequency,halfOfFrequency,halfOfFrequency);

				EditorGUILayout.Space();
			}

			private void DrawSpaceSettings()
			{
				frequencyValue = Mathf.RoundToInt(EditorGUILayout.FloatField("Frequency", frequencyValue));
				scaleOffsetSP.vector3Value = frequencyValue * Vector3.one;

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