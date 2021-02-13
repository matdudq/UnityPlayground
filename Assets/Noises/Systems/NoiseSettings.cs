using UnityEngine;
using Utilities;

namespace Playground.Noises
{
	[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Noise/Create Settings", order = 1)]
	public class NoiseSettings : ScriptableObject
	{
		public FolderReference exportFolder = new FolderReference();

		public int resolution = 256;

		[Range(1,3)]
		public int dimensions = 3;
		[Range(1, 8)]
		public int octaves = 1;
		[Range(1f, 4f)]
		public float lacunarity = 2f;
		[Range(0f, 1f)]
		public float persistence = 0.5f;
		
		public float frequency = 100;

		public NoiseType noiseType = NoiseType.Default;
		
		public Gradient colorGradient = new Gradient();

		public FilterMode filterMode = FilterMode.Point;

	}
}