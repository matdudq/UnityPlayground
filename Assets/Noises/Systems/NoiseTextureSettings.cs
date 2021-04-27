using System;
using UnityEngine;
using Utilities;
using Utilities.Editor;

namespace DudeiNoise
{
	[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Noise/Create Settings", order = 1)]
	public class NoiseTextureSettings : ScriptableObject
	{
		#region Variables
		
		[Tooltip("Folder where textures will be saved.")]
		public FolderReference exportFolder = new FolderReference();

		[Tooltip("Resolution of rendered texture.")]
		public int resolution = 256;
		
		[Tooltip("Says how colors on texture are mapped. Shouldn't be constant color.")]
		public Gradient colorGradient = new Gradient();
		
		[Tooltip("Filter mode of rendered texture. Good to see different ways of filtering to se how noise works.")]
		public FilterMode filterMode = FilterMode.Point;
		
		public NoiseSettings redChannelNoiseSettings = null;
		
		public NoiseSettings greenChannelNoiseSettings = null;
		
		public NoiseSettings blueChannelNoiseSettings = null;
		
		public NoiseSettings alphaChannelNoiseSettings = null;

		#endregion Variables

		#region Public methods

		public NoiseSettings GetNoiseSettingsForChannel(Channel channel)
		{
			switch (channel)
			{
				case Channel.RED:
					return redChannelNoiseSettings;
				case Channel.GREEN:
					return greenChannelNoiseSettings;
				case Channel.BLUE:
					return blueChannelNoiseSettings;
				case Channel.ALPHA:
					return alphaChannelNoiseSettings;
				case Channel.FULL:
					return redChannelNoiseSettings;
			}

			GameConsole.LogError(this, $"Something goes wrong with defined channel {channel}");
			return null;
		}

		#endregion Public methods
	}
}