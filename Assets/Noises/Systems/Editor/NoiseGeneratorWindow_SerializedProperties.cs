﻿#if UNITY_EDITOR
using UnityEditor;

namespace DudeiNoise.Editor
{
    public partial class NoiseGeneratorWindow
    {
        #region Variables - SerializedProperties

        #region Variables - Channels settings

        private SerializedProperty redChanelSettingsProperty = null;
        private SerializedProperty greenChanelSettingsProperty = null;
        private SerializedProperty blueChanelSettingsProperty = null;
        private SerializedProperty alphaChanelSettingsProperty = null;

        #endregion Variables - Channels settings

        #region Variables - Settings SP

        private SerializedProperty positionOffsetSP = null;
        private SerializedProperty rotationOffsetSP = null;
        private SerializedProperty scaleOffsetSP    = null;
			
        private SerializedProperty tillingEnabledSP = null;

        private SerializedProperty dimensionsSP            = null;
        private SerializedProperty octavesSP               = null;
        private SerializedProperty lacunaritySP            = null;
        private SerializedProperty persistenceSP           = null;
        private SerializedProperty woodPatternMultiplierSP = null;
		
        private SerializedProperty turbulenceSP = null;
		
        private SerializedProperty noiseTypeSP = null;
		
        private SerializedProperty tillingPeriodSP  = null;

        #endregion Variables - Settings SP
		
        #endregion Variables - SerializedProperties

        #region Private methods

        private void UpdateActiveNoiseSettingsSp()
        {
	        positionOffsetSP = CurrentNoiseSettingsSP.FindPropertyRelative("positionOffset");
	        rotationOffsetSP = CurrentNoiseSettingsSP.FindPropertyRelative("rotationOffset");
	        scaleOffsetSP = CurrentNoiseSettingsSP.FindPropertyRelative("scaleOffset");
	        tillingEnabledSP = CurrentNoiseSettingsSP.FindPropertyRelative("tillingEnabled");
	        dimensionsSP = CurrentNoiseSettingsSP.FindPropertyRelative("dimensions");
	        octavesSP = CurrentNoiseSettingsSP.FindPropertyRelative("octaves");
	        lacunaritySP = CurrentNoiseSettingsSP.FindPropertyRelative("lacunarity");
	        persistenceSP = CurrentNoiseSettingsSP.FindPropertyRelative("persistence");
	        woodPatternMultiplierSP = CurrentNoiseSettingsSP.FindPropertyRelative("woodPatternMultiplier");
	        turbulenceSP = CurrentNoiseSettingsSP.FindPropertyRelative("turbulence");
	        noiseTypeSP = CurrentNoiseSettingsSP.FindPropertyRelative("noiseType");
	        tillingPeriodSP = CurrentNoiseSettingsSP.FindPropertyRelative("tillingPeriod");
        }

        #endregion Private methods
    }
}
#endif