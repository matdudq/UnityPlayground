﻿using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Procedural
{
    public partial class TerrainGenerator
    {
        private struct GenerateTextureJob : IJobParallelFor
        {
            [ReadOnly]
            public TerrainLayer topTerrainLayer;
            [ReadOnly]
            public NativeArray<TerrainLayer> terrainLayers;
            [ReadOnly]
            public NativeArray<Color> noiseMap;
            
            [WriteOnly]
            public NativeArray<Color32> textureArray;
            
            public void Execute(int index)
            {
                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    if (noiseMap[index].r >= terrainLayers[i].height)
                    {
                        TerrainLayer downLayer = terrainLayers[i];
                        TerrainLayer upLayer = i == terrainLayers.Length - 1  ? topTerrainLayer : terrainLayers[i + 1];
                            
                        float layerHeight = upLayer.height - downLayer.height;
                        float positionOnLayer = noiseMap[index].r - downLayer.height;
                        float currentLayerPositionRatio = positionOnLayer / layerHeight;
                            
                        Color blendedColor = Color.Lerp(downLayer.terrainColor,upLayer.terrainColor, currentLayerPositionRatio);
                        textureArray[index] = blendedColor;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}