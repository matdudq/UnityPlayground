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

		private NoiseTextureSettingsEditor textureSettingsEditor = null;
		
		private NoiseTextureWindow noiseTextureWindow = null;

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
			Vector2 maxWindowSize = new Vector2(500, 800);
			Vector2 minWindowSize = new Vector2(500, 500);
			NoiseGeneratorWindow window = GetWindow<NoiseGeneratorWindow>("Noise Texture Generator");
			window.textureSettingsEditor = (NoiseTextureSettingsEditor)Editor.CreateEditor(noiseTextureSettings);
			window.position = new Rect(0,0,minWindowSize.x, minWindowSize.y);
			window.minSize = minWindowSize;
			window.maxSize = maxWindowSize;
			window.noiseTextureWindow = NoiseTextureWindow.GetWindow(noiseTextureSettings);
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
		
		private void DrawEditorWindow()
		{
			EditorGUI.BeginChangeCheck();

			textureSettingsEditor.DrawCustomInspector();
			
			EditorGUILayout.Space();
			
			if (EditorGUI.EndChangeCheck())
			{
				noiseTextureWindow.RegenerateTexture();
				noiseTextureWindow.Repaint();
			}
			
			if (GUILayout.Button("Save Texture"))
			{
				noiseTextureWindow.SaveTexture();
			}
		}

		#endregion Private methods
	}
}
#endif 