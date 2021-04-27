using System.IO;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace DudeiNoise
{
	public class NoiseTexture
	{
		private NoiseTextureSettings settings = null;

		private Color[] textureValues = null;

		private float[] noiseBuffer = null;
		
		private Texture2D texture = null;
		
		public Texture2D Texture
		{
			get
			{
				return texture;
			}
		}
		
		public NoiseTexture(NoiseTextureSettings generatorSettings)
		{
			this.settings = generatorSettings;
			this.textureValues = new Color[NoiseSettings.maximalResolution * NoiseSettings.maximalResolution];
			this.noiseBuffer = new float[NoiseSettings.maximalResolution * NoiseSettings.maximalResolution];
			texture = new Texture2D(settings.resolution, settings.resolution, TextureFormat.RGBA32, false)
			{
				name = "Noise",
				filterMode = settings.filterMode,
				wrapMode = TextureWrapMode.Clamp
			};
			
			SaveTextureToChannel(Channel.ALPHA);
		}

		public float GetRedChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).r;
		}
		
		public float GetGreenChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).g;
		}
		
		public float GetBlueChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).b;
		}
		
		public float GetAlphaChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).a;
		}
		
		public Color GetProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y);
		}
		
		public void SaveTexture()
		{
			if (settings.exportFolder.IsAssigned)
			{
				File.WriteAllBytes(ConstructSavePath(), texture.EncodeToPNG());
				AssetDatabase.Refresh();
			}
			else
			{
				GameConsole.LogError(this, "Cannot save texture! Export folder not set up.");
			}
		}
		
		public void SaveTextureToChannel(Channel channel)
		{
			if (texture.width != settings.resolution)
			{
				texture.Resize( settings.resolution, settings.resolution);
			}

			if (texture.filterMode != settings.filterMode)
			{
				texture.filterMode = settings.filterMode;
			}
			
			switch (channel)
			{
				case Channel.RED:
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.redChannelNoiseSettings, settings.resolution);
					break;
				case Channel.GREEN:
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.greenChannelNoiseSettings,settings.resolution);
					break;
				case Channel.BLUE:
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.blueChannelNoiseSettings,settings.resolution);
					break;
				case Channel.ALPHA:
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.alphaChannelNoiseSettings,settings.resolution);
					break;
				case Channel.FULL:
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.redChannelNoiseSettings, settings.resolution);
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.greenChannelNoiseSettings,settings.resolution);
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.blueChannelNoiseSettings,settings.resolution);
					Noise.GenerateNoiseTexture(ref noiseBuffer,settings.alphaChannelNoiseSettings,settings.resolution);
					break;
			}
			
			for (int y = 0; y < settings.resolution; y++)
			{
				for (int x = 0; x < settings.resolution; x++)
				{
					switch (channel)
					{
						case Channel.RED:
							textureValues[y * settings.resolution + x].r = noiseBuffer[y * settings.resolution + x];
							break;
						case Channel.GREEN:
							textureValues[y * settings.resolution + x].g = noiseBuffer[y * settings.resolution + x];
							break;
						case Channel.BLUE:
							textureValues[y * settings.resolution + x].b = noiseBuffer[y * settings.resolution + x];
							break;
						case Channel.ALPHA:
							textureValues[y * settings.resolution + x].a = noiseBuffer[y * settings.resolution + x];
							break;
						case Channel.FULL:
							textureValues[y * settings.resolution + x].r = noiseBuffer[y * settings.resolution + x];
							textureValues[y * settings.resolution + x].g = noiseBuffer[y * settings.resolution + x];
							textureValues[y * settings.resolution + x].b = noiseBuffer[y * settings.resolution + x];
							textureValues[y * settings.resolution + x].a = noiseBuffer[y * settings.resolution + x]; 
							break;
					}
				}
			}
			
			texture.SetPixels(textureValues);
			texture.Apply();
		}
		
		private string ConstructSavePath()
		{
			return settings.exportFolder.Path + $"/Red_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
											  + $"Green_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
											  + $"Blue_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
											  + $"Alpha_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
											  + $"_Noise_{settings.resolution}.png";
		}
	}
}