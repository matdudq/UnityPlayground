using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Procedural
{
    public partial class TerrainGenerator
    {
        private struct GenerateTextureJob : IJobParallelFor
        {
            public int width;
            public int height;

            public TerrainLayer topTerrainLayer;
            
            public NativeArray<TerrainLayer> terrainLayers;
            public NativeArray<Color> noiseTexture;
            
            public NativeArray<Color32> textureArray;
            
            public void Execute(int index)
            {
                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    if (noiseTexture[index].r >= terrainLayers[i].height)
                    {
                        TerrainLayer downLayer = terrainLayers[i];
                        TerrainLayer upLayer = i == terrainLayers.Length - 1  ? topTerrainLayer : terrainLayers[i + 1];
                            
                        float layerHeight = upLayer.height - downLayer.height;
                        float positionOnLayer = noiseTexture[index].r - downLayer.height;
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