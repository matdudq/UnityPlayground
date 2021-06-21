using DudeiNoise;
using UnityEngine;

namespace Procedural
{
    [CreateAssetMenu(fileName = nameof(TerrainDefinition), menuName = "Procedural/" + nameof(TerrainDefinition), order = 1)]
    public class TerrainDefinition : ScriptableObject
    {
        [SerializeField] 
        private TerrainLayer[] terrainLayers = null;

        [SerializeField] 
        private NoiseTextureSettings textureSettings = null;

        [SerializeField] 
        private NoiseTextureChannel activeChanel = NoiseTextureChannel.RED;

        [SerializeField]
        private int chunkResolution = 100;
        
        [SerializeField]
        private float chunkSize = 10.0f;

        [SerializeField] 
        private float heightRange = 10.0f;

        [SerializeField] 
        private AnimationCurve heightCurve = null;
        
        [SerializeField]
        private Vector3 terrainOffset = Vector3.zero;
        
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

        public Vector3 TerrainOffset => terrainOffset;

        public int ChunkResolution => chunkResolution;

        public AnimationCurve HeightCurve => heightCurve;
    }
}