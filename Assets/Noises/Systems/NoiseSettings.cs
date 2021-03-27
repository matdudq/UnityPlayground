using UnityEngine;
using Utilities.Editor;

namespace Noises
{
	[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Noise/Create Settings", order = 1)]
	public class NoiseSettings : ScriptableObject
	{
		#region Variables

		[Tooltip("Folder where textures will be saved.")]
		public FolderReference exportFolder = new FolderReference();

		[Tooltip("Resolution of rendered texture.")]
		public int resolution = 256;
		[Tooltip("Scales range of noise.")]
		public float frequency = 100;
		
		[Tooltip("You can move though noise surface by that. Position offset in 'noise-space'.")]
		public Vector3 positionOffset = Vector3.zero;
		[Tooltip("You can rotate noise surface by that to get more interesting results. Rotation offset in 'noise-space'.")]
		public Vector3 rotationOffset = Vector3.zero;
		
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
		
		[Tooltip("Says how colors on texture are mapped. Shouldn't be constant color.")]
		public Gradient colorGradient = new Gradient();
		
		[Tooltip("Filter mode of rendered texture. Good to see different ways of filtering to se how noise works.")]
		public FilterMode filterMode = FilterMode.Point;


		#endregion Variables
	}
}