using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow
	{
		private bool stylesInitialized = false;
		
		private GUIStyle sectionStyle;
		
		private void InitializeStyles()
		{
			
			sectionStyle = new GUIStyle(EditorStyles.helpBox);
		}
		
	}
}