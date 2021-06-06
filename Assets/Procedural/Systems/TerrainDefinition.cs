using System;
using DudeiNoise;
using UnityEngine;

namespace Procedural
{
    [Serializable]
    public class TerrainDefinition
    {
        [SerializeField] 
        private TerrainLayer[] terrainLayers = null;

        [SerializeField] 
        private NoiseTextureSettings textureSettings = null;

        [SerializeField] 
        private NoiseTextureChannel activeChanel = NoiseTextureChannel.RED;

        [SerializeField]
        private float chunkSize = 10.0f;

        [SerializeField] 
        private float heightRange = 10.0f;
        
        public TerrainLayer[] TerrainLayers
        {
            get
            {
                return terrainLayers;
            }
        }

        public NoiseSettings NoiseSettings
        {
            get
            {
                return TextureSettings.GetNoiseSettingsForChannel(activeChanel);
            }
        }

        public NoiseTextureSettings TextureSettings
        {
            get
            {
                return textureSettings;
            }
        }

        public float ChunkSize
        {
            get
            {
                return chunkSize;
            }
        }

        public float HeightRange => heightRange;
    }
}