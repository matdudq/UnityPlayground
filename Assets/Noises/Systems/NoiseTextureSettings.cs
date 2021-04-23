using System;
using UnityEditor;
using UnityEngine;
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
		
		public NoiseSettings noiseSettings = null;

		#endregion Variables
	}
}