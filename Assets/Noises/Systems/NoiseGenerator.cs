using Unity.Collections;
using UnityEngine;

namespace Playground.Noises
{
	[RequireComponent(typeof(MeshRenderer))]
	public partial class NoiseGenerator : MonoBehaviour
	{
		[SerializeField,ReadOnly]
		private MeshRenderer meshRenderer = null;
		
		public int resolution = 256;

		[Range(1,3)]
		public int dimentions = 3;
		
		[Range(1, 8), Tooltip("During using sum function represents number of sum iteration")]
		public int octaves = 1;
		
		[Range(1f, 4f), Tooltip("Informs how much frequency changes during each iteration inside octave.")]
		public float lacunarity = 2f;

		[Range(0f, 1f), Tooltip("Informs how much amplitude sample changes during each iteration inside octave.")]
		public float persistence = 0.5f;
		
		[Tooltip("Parameter which changes frequency of noise, which basically scales up input vector")]
		public float frequency = 100;

		public NoiseType noiseType = NoiseType.Default;
		
		private Texture2D noiseTexture = null;

		public Gradient colorGradient = null;
		
		private void Awake()
		{
			Initialize();
		}

		private void Update()
		{
			if (transform.hasChanged)
			{
				transform.hasChanged = false;
				GenerateNoise();
			}
		}

		private void Initialize()
		{
			meshRenderer = GetComponent<MeshRenderer>();
			//Editor case
			if (noiseTexture == null)
			{
				noiseTexture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true)
				{
					name = "Procedural Noise",
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Clamp
				};

				meshRenderer.sharedMaterial.mainTexture = noiseTexture;
			}
			
			GenerateNoise();
		}

		private void GenerateNoise()
		{
			if (noiseTexture.width != resolution)
			{
				noiseTexture.Resize(resolution,resolution);
			}
			 
			Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
			Vector3 point10 = transform.TransformPoint(new Vector3(0.5f,-0.5f));
			Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f,0.5f));
			Vector3 point11 = transform.TransformPoint(new Vector3(0.5f,0.5f));
			
			float stepSize = 1.0f / resolution;

			NoiseMethod noise = Noise.methods[(int)noiseType][dimentions - 1];
			
			for (int y = 0; y < resolution; y++)
			{
				Vector3 point0 = Vector3.Lerp(point00,point01, (y + 0.5f) * stepSize);
				Vector3 point1 = Vector3.Lerp(point10,point11, (y + 0.5f) * stepSize);
				
				for (int x = 0; x < resolution; x++)
				{
					Vector3 point = Vector3.Lerp(point0,point1, (x + 0.5f) * stepSize);
					float sample = Noise.Sum(noise, point, frequency, octaves, lacunarity, persistence);
					if (noiseType == NoiseType.Perlin) {
						sample = sample * 0.5f + 0.5f;
					}
					noiseTexture.SetPixel(x,y,colorGradient.Evaluate(sample));
				}
			}
			noiseTexture.Apply();
		}
	}

}