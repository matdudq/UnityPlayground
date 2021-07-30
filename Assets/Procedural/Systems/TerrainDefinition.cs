﻿using DudeiNoise;
using UnityEngine;

namespace Procedural
{
    [CreateAssetMenu(fileName = nameof(TerrainDefinition), menuName = "Procedural/" + nameof(TerrainDefinition), order = 1)]
    public class TerrainDefinition : ScriptableObject
    {
        public const int MAP_CHUNK_SIZE = 97;
        
        [Header("Terrain preview definition")]
        [SerializeField] 
        private TerrainLayer[] terrainLayers = null;

        [SerializeField] 
        private NoiseSettings noiseSettings = null;
        
        [SerializeField, Range(0,4)]
        private int levelOfDetails = 1;
        
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
                return noiseSettings;
            }
        }

        public float ChunkSize
        {
            get
            {
                return chunkSize;
            }
        }

        public float HeightRange
        {
            get
            {
                return heightRange;
            }
        }

        public Vector3 TerrainOffset
        {
            get
            {
                return terrainOffset;
            }
        }

        public AnimationCurve HeightCurve
        {
            get
            {
                return heightCurve;
            }
        }

        public int LevelOfDetails
        {
            get
            {
                return levelOfDetails;
            }
        }
    }
}