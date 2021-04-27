#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Editor;
using Utilities;

namespace DudeiNoise
{
	public partial class NoiseGeneratorWindow : EditorWindow
	{
		#region Variables

		private NoiseTextureSettingsEditor textureSettingsEditor = null;
		private NoiseTextureSettings settings = null;
		
		private TextureWindow textureWindow = null;
		
		private NoiseTexture noiseTexture = null;

		private Color[] redChanelTextureArray = null;
		private Color[] greenChanelTextureArray = null;
		private Color[] blueChanelTextureArray = null;
		private Color[] alphaChanelTextureArray = null;

		private SerializedProperty redChanelSettingsProperty = null;
		private SerializedProperty greenChanelSettingsProperty = null;
		private SerializedProperty blueChanelSettingsProperty = null;
		private SerializedProperty alphaChanelSettingsProperty = null;
		
		private Dictionary<Type,INoiseGeneratorWindowTab> tabs = null;
		private INoiseGeneratorWindowTab activeTab = null;
		
		private Channel activeChannel = Channel.ALPHA;
		
		#endregion Variables

		#region Properties

		private NoiseSettings CurrentNoiseSettings
		{
			get
			{
				return settings.GetNoiseSettingsForChannel(activeChannel);
			}
		}

		private SerializedProperty CurrentNoiseSettingsSP
		{
			get
			{
				switch (activeChannel)
				{
					case Channel.RED:
						return redChanelSettingsProperty;
					case Channel.GREEN:
						return greenChanelSettingsProperty;
					case Channel.BLUE:
						return blueChanelSettingsProperty;
					case Channel.ALPHA:
						return alphaChanelSettingsProperty;
					case Channel.FULL:
						return redChanelSettingsProperty;
				}
				
				GameConsole.LogError(this, $"Something goes wrong with defined channel {activeChannel}");
				return null;
			}
		}
		
		#endregion Properties
		
		#region Unity methods

		private void OnGUI()
		{
			DrawEditorWindow();
		}

		private void OnDestroy()
		{
			textureWindow.Close();
		}

		#endregion Unity methods
		
		#region Public methods

		public static void Open(NoiseTextureSettings settings)
		{ 
			Vector2 maxWindowSize = new Vector2(500, 800);
			Vector2 minWindowSize = new Vector2(500, 500);
			
			NoiseGeneratorWindow window = GetWindow<NoiseGeneratorWindow>("Noise Texture Generator");
			
			window.position = new Rect(0,0,minWindowSize.x, minWindowSize.y);
			window.minSize = minWindowSize;
			window.maxSize = maxWindowSize;

			window.Initialize(settings);
			
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

		private void Initialize(NoiseTextureSettings settings)
		{
			this.settings = settings;
			noiseTexture = new NoiseTexture(settings);
			
			textureSettingsEditor = (NoiseTextureSettingsEditor)Editor.CreateEditor(settings);

			redChanelSettingsProperty = textureSettingsEditor.serializedObject.FindProperty("redChannelNoiseSettings");
			greenChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("greenChannelNoiseSettings");
			blueChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("blueChannelNoiseSettings");
			alphaChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("alphaChannelNoiseSettings");
			
			
			textureWindow = TextureWindow.GetWindow(new Vector2(500,0),"Noise Texture", settings.resolution, settings.filterMode);
			redChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			greenChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			blueChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			alphaChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			
			tabs = new Dictionary<Type, INoiseGeneratorWindowTab>()
			{
				{typeof(CustomSpaceTab), new CustomSpaceTab(this)},
				{typeof(TillingTab), new TillingTab(this)}
			};
			
			SwitchTab(typeof(CustomSpaceTab));
			
			RegenerateTextures();

		}
		
		private void SwitchTab(Type tab)
		{
			if (activeTab == tabs[tab])
			{
				return;
			}
			
			activeTab?.OnTabExit();

			activeTab = tabs[tab];

			activeTab?.OnTabEnter();
		}
		
		private void SwitchTab(INoiseGeneratorWindowTab tab)
		{
			if (activeTab == tab)
			{
				return;
			}
			
			activeTab?.OnTabExit();

			activeTab = tab;

			activeTab?.OnTabEnter();
		}
		
		private void DrawEditorWindow()
		{
			textureSettingsEditor.DrawCustomInspector();
			
			DrawWindowTabs();
		
			if (EditorGUI.EndChangeCheck())
			{
				textureSettingsEditor.serializedObject.ApplyModifiedProperties();	
				EditorUtility.SetDirty(settings);
			}

			DrawSaveAndChannelsButtons();
		}

		private void DrawWindowTabs()
		{
			GUILayout.BeginHorizontal();

			foreach (INoiseGeneratorWindowTab tab in tabs.Values)
			{
				if (tab.DrawButton())
				{
					SwitchTab(tab);
				}
			}
			
			GUILayout.EndHorizontal();
			
			activeTab.DrawInspector();
			
			EditorGUILayout.Space();
		}

		private void DrawSaveAndChannelsButtons()
		{
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Red channel"))
			{
				activeChannel = Channel.RED;
			}
			
			if (GUILayout.Button("Blue channel"))
			{
				activeChannel = Channel.BLUE;
			}
			
			if (GUILayout.Button("Green channel"))
			{
				activeChannel = Channel.GREEN;
			}
			
			if (GUILayout.Button("Alpha channel"))
			{
				activeChannel = Channel.ALPHA;
			}
			
			if (GUILayout.Button("Full channel"))
			{
				activeChannel = Channel.FULL;
			}
			
			GUILayout.EndHorizontal();
			
			if (GUILayout.Button("Save Texture"))
			{
				noiseTexture.SaveTexture();
			}
		}
		
		private void RegenerateTextures()
		{
			noiseTexture.SaveTextureToChannel(activeChannel);
			
			RegenerateCachedChanelTextures();
				
			textureWindow.Repaint();
		}
		
		private void RegenerateCachedChanelTextures()
		{
			Color[] originalTextureArray = noiseTexture.Texture.GetPixels();
			
			for (int y = 0; y < settings.resolution; y++)
			{
				for (int x = 0; x < settings.resolution; x++)
				{
					Color original = originalTextureArray[y * settings.resolution + x];
					
					Color redChanel = new Color(original.r,original.r,original.r,1.0f);
					Color greenChanel = new Color(original.g,original.g,original.g,1.0f);
					Color blueChanel = new Color(original.b,original.b,original.b,1.0f);
					Color alphaChanel = new Color(original.a,original.a,original.a,1.0f);
					
					redChanelTextureArray[y * settings.resolution + x] = redChanel;
					greenChanelTextureArray[y * settings.resolution + x] = greenChanel;
					blueChanelTextureArray[y * settings.resolution + x] = blueChanel;
					alphaChanelTextureArray[y * settings.resolution + x] = alphaChanel;
				}
			}

			switch (activeChannel)
			{
				case Channel.RED:
					textureWindow.UpdateTexture(redChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case Channel.GREEN:
					textureWindow.UpdateTexture(greenChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case Channel.BLUE:
					textureWindow.UpdateTexture(blueChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case Channel.ALPHA:
					textureWindow.UpdateTexture(alphaChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case Channel.FULL:
					textureWindow.UpdateTexture(originalTextureArray, settings.resolution, settings.filterMode);
					break;
			}
		}

		#endregion Private methods
	}
}
#endif 