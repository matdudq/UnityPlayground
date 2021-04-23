namespace DudeiNoise
{
	public static class NoiseSettingsExtension
	{
		public static NoiseMethod NoiseMethod(this NoiseSettings generatorSettings)
		{
			return Noise.methods[(int) generatorSettings.noiseType][generatorSettings.dimensions - 1];
		}
	}
}