using UnityEditor;
using UnityEngine;

namespace Playground.Noises
{
	[CustomEditor(typeof(NoiseSettings))]
	public class NoiseSettingsCustomEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Generator"))
			{
				NoiseGeneratorWindow.Open(target as NoiseSettings);
			}
		}
	}
}