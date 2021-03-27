using System.IO;
using UnityEditor;
using UnityEngine;
using Utilities.Editor;
using Utilities;

namespace Noises
{
	public class NoiseGeneratorWindow : ExtendedEditorWindow
	{
		#region Variables

		private NoiseSettings settings            = null;
		private Texture2D     currentNoiseTexture = null;

		#endregion Variables

		#region Unity methods

		private void OnGUI()
		{
			DrawEditorWindow();
		}

		#endregion Unity methods
		
		#region Public methods

		public static void Open(NoiseSettings noiseSettings)
		{
			Vector2 windowSize = new Vector2(400, 800);
			
			NoiseGeneratorWindow window = GetWindow<NoiseGeneratorWindow>("Noise Texture Generator");
			window.settings = noiseSettings;
			window.serializedObject = new SerializedObject(noiseSettings);
			window.position = new Rect(0,0,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;
			window.CreateNewTexture();
			window.Show();
		}

		#endregion Public methods

		#region Private methods

		private void CreateNewTexture()
		{
			//Editor case
			if (currentNoiseTexture == null)
			{
				currentNoiseTexture = new Texture2D(settings.resolution, settings.resolution, TextureFormat.RGB24, true)
				{
					name = "Noise",
					filterMode = settings.filterMode,
					wrapMode = TextureWrapMode.Clamp
				};
			}
			
			Noise.GenerateTextureNoise(ref currentNoiseTexture,settings);
		}
		
		private void DrawEditorWindow()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			
			currentProperty = serializedObject.GetIterator();
			DrawProperties(currentProperty,false);
			ApplyChanges();
			
			EditorGUILayout.Space();
			
			if (EditorGUI.EndChangeCheck())
			{
				Noise.GenerateTextureNoise(ref currentNoiseTexture,settings);
			}
			
			EditorGUILayout.BeginVertical();
			
			EditorGUI.PrefixLabel(new Rect(30, 385, 100, 15), 0, new GUIContent("Preview:"));
			EditorGUI.DrawPreviewTexture(new Rect(30, 400, 340, 340), currentNoiseTexture);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical();

			if (GUILayout.Button("Save Texture"))
			{
				if ( settings.exportFolder.IsAssigned)
				{
					string path =  settings.exportFolder.Path + $"/{settings.noiseType}{settings.dimensions}D_Noise_{settings.resolution}.png";
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