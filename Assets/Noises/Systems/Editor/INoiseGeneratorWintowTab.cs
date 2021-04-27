namespace DudeiNoise
{
	public partial class NoiseGeneratorWindow 
	{
		private interface INoiseGeneratorWindowTab
		{
			void OnTabEnter();

			void OnTabExit();

			void DrawInspector();

			bool DrawButton();
		}
	}
}