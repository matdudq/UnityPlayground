using System.IO;
using UnityEditor;
using UnityEngine;

namespace Playground.Noises
{
	public class NoiseGeneratorWindow : EditorWindow
	{
		public FolderReference exportFolder;
		
		private int resolution = 256;

		[Range(1,3)]
		private int dimentions = 3;
		
		[Range(1, 8), Tooltip("During using sum function represents number of sum iteration")]
		private int octaves = 1;
		
		[Range(1f, 4f), Tooltip("Informs how much frequency changes during each iteration inside octave.")]
		private float lacunarity = 2f;

		[Range(0f, 1f), Tooltip("Informs how much amplitude sample changes during each iteration inside octave.")]
		private float persistence = 0.5f;
		
		[Tooltip("Parameter which changes frequency of noise, which basically scales up input vector")]
		private float frequency = 100;

		private NoiseType noiseType = NoiseType.Default;
		
		public Gradient colorGradient = null;

		private Texture2D currentNoiseTexture = null;
		
		[MenuItem("Noise Texture Generator", menuItem = "Window/Noise Texture Generator")]
		private static void ShowWindow()
		{
			Vector2 windowSize = new Vector2(400, 800);
			
			NoiseGeneratorWindow window = GetWindow<NoiseGeneratorWindow>("Noise Texture Generator");
			window.position = new Rect(0,0,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;
			window.Show();
		}

		private void OnEnable()
		{
			//resolutionContent = new GUIContent();
			
			
			Initialize();
		}

		private void Initialize()
		{
			//Editor case
			if (currentNoiseTexture == null)
			{
				currentNoiseTexture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true)
				{
					name = "Noise",
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Clamp
				};
			}
		}
		
		private void OnGUI()
		{
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			
			resolution = EditorGUILayout.IntField("Resolution",resolution);
			dimentions = EditorGUILayout.IntField("Dimentions", dimentions);			
			frequency = EditorGUILayout.FloatField("Frequency", frequency);		
			octaves = EditorGUILayout.IntField("Octaves", octaves);			
			lacunarity = EditorGUILayout.FloatField("Lacunarity", lacunarity);			
			persistence = EditorGUILayout.FloatField("Persistence", persistence);
			noiseType = (NoiseType)EditorGUILayout.EnumFlagsField("NoiseType",noiseType);
			//colorGradient = EditorGUILayout.GradientField("Noise color gradient", colorGradient);
			
			EditorGUILayout.Space();

			if (EditorGUI.EndChangeCheck())
			{
				GenerateNoise();
			}
			
			EditorGUILayout.BeginVertical();
			
			EditorGUI.PrefixLabel(new Rect(30, 185, 100, 15), 0, new GUIContent("Preview:"));
			EditorGUI.DrawPreviewTexture(new Rect(30, 200, 340, 340), currentNoiseTexture);
			
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical();

			if (GUILayout.Button("Generate Texture"))
			{
				if (exportFolder.IsAssigned)
				{
					string path = exportFolder.Path + $"/{prefab.name}{generationSetup.suffix}.png";
					File.WriteAllBytes(path, currentNoiseTexture.EncodeToPNG());
				}
			}
			
			EditorGUILayout.EndVertical();
		}
		
		private void GenerateNoise()
		{
			if (currentNoiseTexture.width != resolution)
			{
				currentNoiseTexture.Resize(resolution,resolution);
			}
			 
			// Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
			// Vector3 point10 = transform.TransformPoint(new Vector3(0.5f,-0.5f));
			// Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f,0.5f));
			// Vector3 point11 = transform.TransformPoint(new Vector3(0.5f,0.5f));
			
			Vector3 point00 = new Vector3(-0.5f,-0.5f);
			Vector3 point10 = new Vector3(0.5f, -0.5f);
			Vector3 point01 = new Vector3(-0.5f,0.5f);
			Vector3 point11 = new Vector3(0.5f,0.5f);
			
			float stepSize = 1.0f / resolution;

			NoiseMethod noise = Noise.methods[(int)noiseType][dimentions - 1];
			
			 string title = $"Busy {timespan.TotalMinutes.ToString("00")}:{(timespan.TotalSeconds % 60).ToString("00")}";
                            string info = $"Rendering sprites... {i}/{prefabs.Length} {prefab.name}";
                            float progress = (float)i / prefabs.Length;
            
                            if (EditorUtility.DisplayCancelableProgressBar(title, info , progress))
                            {
                                //Cancel was pressed.
                                break;
                            }  
			
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
					currentNoiseTexture.SetPixel(x,y,colorGradient.Evaluate(sample));
				}
			}
			
			currentNoiseTexture.Apply();
		}
	}
}