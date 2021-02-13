using UnityEditor;
using UnityEngine;

namespace Noises
{
	[CustomEditor(typeof(NoiseSettings))]
	public class NoiseSettingsCustomEditor : Editor
	{
		#region Unity methods

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Generator"))
			{
				NoiseGeneratorWindow.Open(target as NoiseSettings);
			}
		}

		#endregion Unity methods
	}
}