#if UNITY_EDITOR
namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow 
	{
		private interface INoiseGeneratorWindowTab
		{
			void OnTabEnter();

			void OnTabExit();

			void OnChannelChange();
			
			void DrawInspector();

			bool DrawButton();
		}
	}
}
#endif