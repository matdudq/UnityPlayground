using UnityEditor;
using UnityEngine;
using Utilities.Editor;

namespace DudeiNoise
{
	[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Noise/Create Settings", order = 1)]
	public class NoiseTextureSettings : ScriptableObject
	{
		#if UNITY_EDITOR
		[CustomEditor(typeof(NoiseTextureSettings))]
    	private class NoiseSettingsEditor : Editor
    	{
    		#region Unity methods
    
    		public override void OnInspectorGUI()
    		{
    			if (GUILayout.Button("Open Generator"))
    			{
    				NoiseGeneratorWindow.Open(target as NoiseTextureSettings);
    			}
    		}
    
    		#endregion Unity methods
    	}
		#endif
		
		#region Variables

		[Header("Texture settings")]
		[Tooltip("Folder where textures will be saved.")]
		public FolderReference exportFolder = new FolderReference();

		[Tooltip("Resolution of rendered texture.")]
		public int resolution = 256;
		
		[Tooltip("You can move though noise surface by that. Position offset in 'noise-space'.")]
		public Vector3 positionOffset = Vector3.zero;
		[Tooltip("You can rotate noise surface by that to get more interesting results. Rotation offset in 'noise-space'.")]
		public Vector3 rotationOffset = Vector3.zero;
		
		[Tooltip("Says how colors on texture are mapped. Shouldn't be constant color.")]
		public Gradient colorGradient = new Gradient();
		
		[Tooltip("Filter mode of rendered texture. Good to see different ways of filtering to se how noise works.")]
		public FilterMode filterMode = FilterMode.Point;
		
		public NoiseSettings noiseSettings = null;

		#endregion Variables
	}
}