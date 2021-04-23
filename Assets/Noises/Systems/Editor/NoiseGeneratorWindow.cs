#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Utilities.Editor;
using Utilities;

namespace DudeiNoise
{
	public class NoiseGeneratorWindow : EditorWindow
	{
		#region Variables

		private Editor textureSettingsEditor = null;
		private NoiseTextureSettings textureSettings            = null;
		private Texture2D     currentNoiseTexture = null;

		#endregion Variables

		#region Unity methods

		private void OnGUI()
		{
			DrawEditorWindow();
		}

		#endregion Unity methods
		
		#region Public methods

		public static void Open(NoiseTextureSettings noiseTextureSettings)
		{
			Vector2 windowSize = new Vector2(400, 800);
			
			NoiseGeneratorWindow window = GetWindow<NoiseGeneratorWindow>("Noise Texture Generator");
			window.textureSettings = noiseTextureSettings;
			window.textureSettingsEditor = Editor.CreateEditor(noiseTextureSettings);
			window.position = new Rect(0,0,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;
			window.CreateNewTexture();
			window.Show();
		}

		#endregion Public methods

		#region Private methods

 		[MenuItem("Noise Generator Window", menuItem = "Tools/Noise Generator Window")]
        private static void ShowWindow()
        {
            List<NoiseTextureSettings> contentDownloaders = EditorGameExtensions.LoadProjectAssetsByType<NoiseTextureSettings>();

			NoiseTextureSettings newPreset = contentDownloaders.Count == 0 ? null : contentDownloaders[0];

            if (newPreset == null)
            {
                GameConsole.LogWarning(GetWindow<NoiseGeneratorWindow>() ,"Lightmapping Helper setup object have to be defined once inside a project. Creating deafult one ...");

                newPreset = CreateInstance<NoiseTextureSettings>();
                AssetDatabase.CreateAsset(newPreset, "Assets/Lightmapping-Helper-Preset.asset");

                EditorUtility.SetDirty(newPreset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Open(newPreset);
        }

		private void CreateNewTexture()
		{
			//Editor case
			if (currentNoiseTexture == null)
			{
				currentNoiseTexture = new Texture2D(textureSettings.resolution, textureSettings.resolution, TextureFormat.RGB24, true)
				{
					name = "Noise",
					filterMode = textureSettings.filterMode,
					wrapMode = TextureWrapMode.Clamp
				};
			}
			
			Noise.GenerateTextureNoise(ref currentNoiseTexture,textureSettings);
		}
		
		private void DrawEditorWindow()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();

			textureSettingsEditor.DrawDefaultInspector();
			
			EditorGUILayout.Space();
			
			if (EditorGUI.EndChangeCheck())
			{
				Noise.GenerateTextureNoise(ref currentNoiseTexture,textureSettings);
			}
			
			EditorGUILayout.BeginVertical();
			
			EditorGUI.PrefixLabel(new Rect(30, 430, 100, 15), 0, new GUIContent("Preview:"));
			EditorGUI.DrawPreviewTexture(new Rect(30, 445, 340, 340), currentNoiseTexture);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical();

			if (GUILayout.Button("Save Texture"))
			{
				if ( textureSettings.exportFolder.IsAssigned)
				{
					string path =  textureSettings.exportFolder.Path + $"/{textureSettings.noiseSettings.noiseType}{textureSettings.noiseSettings.dimensions}D_Noise_{textureSettings.resolution}.png";
					File.WriteAllBytes(path, currentNoiseTexture.EncodeToPNG());
					AssetDatabase.Refresh();
				}
				else
				{
					GameConsole.LogError(this, "Cannot save texture! Export folder not set up.");
				}
			}
			
			EditorGUILayout.EndVertical();
		}

		#endregion Private methods
	}
}
#endif 