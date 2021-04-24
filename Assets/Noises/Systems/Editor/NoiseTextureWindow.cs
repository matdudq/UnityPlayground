using System.IO;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace DudeiNoise
{
	public class NoiseTextureWindow : EditorWindow
	{
		private Texture2D currentTexture = null;

		private NoiseTextureSettings settings = null;
		
		private static Vector2 windowSize = new Vector2(400, 400);

		public bool IsTextureSetUp
		{
			get
			{
				return currentTexture != null;
			}
		}

		public void RegenerateTexture()
		{
			Noise.GenerateTextureNoise(ref currentTexture,settings);
		}

		public void SaveTexture()
		{
			if (settings.exportFolder.IsAssigned)
			{
				string path =  settings.exportFolder.Path + $"/{settings.noiseSettings.noiseType}{settings.noiseSettings.dimensions}D_Noise_{settings.resolution}.png";
				File.WriteAllBytes(path, currentTexture.EncodeToPNG());
				AssetDatabase.Refresh();
			}
			else
			{
				GameConsole.LogError(this, "Cannot save texture! Export folder not set up.");
			}
		}
		
		public static NoiseTextureWindow GetWindow(NoiseTextureSettings noiseTextureSettings)
		{
			NoiseTextureWindow window = GetWindow<NoiseTextureWindow>("Noise Texture Preview");
			window.position = new Rect(500,0,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;
			window.settings = noiseTextureSettings;
			window.CreateNewTexture();
			window.Show();

			return window;
		}

		private void CreateNewTexture()
		{
			//Editor case
			if (!IsTextureSetUp)
			{
				currentTexture = new Texture2D(settings.resolution, settings.resolution, TextureFormat.RGB24, true)
				{
					name = "Noise",
					filterMode = settings.filterMode,
					wrapMode = TextureWrapMode.Clamp
				};
			}
			RegenerateTexture();
		}
		
		private void OnGUI()
		{
			Rect displayArea = new Rect(0, 0, position.width, position.height);
			
			EditorGUILayout.BeginVertical();
			GUILayout.BeginArea(displayArea);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();


			EditorGUI.DrawPreviewTexture(displayArea,currentTexture);

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
			
			
			EditorGUILayout.EndVertical();
		}
	}
}