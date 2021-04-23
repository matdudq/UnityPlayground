using System;
using UnityEngine;

namespace DudeiNoise
{
	[Serializable]
	public class NoiseSettings
	{
		[Tooltip("Scales range of noise.")]
		public float frequency = 100;
		[Range(1,3), Tooltip("Dimension in which noise will be generated.")]
		public int dimensions = 3;
		[Range(1, 8), Tooltip("Octaves represents count of sum iteration during generating noise.")]
		public int octaves = 1;
		[Range(1f, 4f), Tooltip("Lacunarity is multiplier for texture frequency during each sum iteration.")]
		public float lacunarity = 2f;
		[Range(0f, 1f), Tooltip("Persistence is multiplier for amplitude value during each sum iteration.")]
		public float persistence = 0.5f;
		[Range(1f, 100f), Tooltip("When set up to 1 leaves sample as it is. When larger than 1 allows us to get wood like pattern.")]
		public float woodPatternMultiplier = 1.0f;
		
		public bool turbulence = false;
		
		[Tooltip("Defines method of generating.")]
		public NoiseType noiseType = NoiseType.Default;
	}
}