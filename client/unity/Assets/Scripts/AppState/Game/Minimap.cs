using System;
using System.Collections.Generic;
using KG;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace Game
{
    public class Minimap
    {
        private Texture2D _staticStaticTexture;
        private RenderTexture _completeRT;
        public Texture minimapTexture => _completeRT;

        public void LoadMapData(Vector2 pixelSize, KG.Map map, int mergeCellSize, float scaler)
        {
            var scaledMergeCellSize = Mathf.Max(1, Mathf.RoundToInt(mergeCellSize * scaler));
            LoadMapData(map, scaledMergeCellSize);
            OnResize(pixelSize);
        }

        private void LoadMapData(KG.Map map, int mergeCellSize)
        {
            var cellWidth = map.width / mergeCellSize;
            var cellHeight = map.height / mergeCellSize;

            var staticCells = new Color32[cellWidth * cellHeight];
            Span<bool> walkables = stackalloc bool[mergeCellSize * mergeCellSize];
            for (var cellYIndex = 0; cellYIndex < cellHeight; cellYIndex++)
            for (var cellXIndex = 0; cellXIndex < cellWidth; cellXIndex++)
            {
                walkables.Clear();
                var validCount = 0;
            
                for (var localYIndex = 0; localYIndex < mergeCellSize; localYIndex++)
                for (var localXIndex = 0; localXIndex < mergeCellSize; localXIndex++)
                {
                    if (cellXIndex * mergeCellSize + localXIndex >= map.width || 
                        cellYIndex * mergeCellSize + localYIndex >= map.height)
                        continue;
                    
                    var bufferIndex = localYIndex * mergeCellSize + localXIndex;
                    var mapIndex = ((cellYIndex * mergeCellSize + localYIndex) * map.width) +
                                   (cellXIndex * mergeCellSize + localXIndex);

                    validCount++;
                    walkables[bufferIndex] = map._walkability[mapIndex];
                }

                staticCells[cellYIndex + (cellWidth - 1 - cellXIndex) * cellHeight] = ToColor32(walkables, validCount);
            }

            _staticStaticTexture = new Texture2D(map.width / mergeCellSize, map.height / mergeCellSize, TextureFormat.RGBA32, false);
            _staticStaticTexture.name = $"{nameof(Minimap)} Texture";
            _staticStaticTexture.filterMode = FilterMode.Point;
            _staticStaticTexture.SetPixels32(staticCells);
            _staticStaticTexture.Apply();
        }

        private static Color32 ToColor32(Span<bool> walkables, int validCount)
        {
            var count = 0;
            for (var i = 0; i < walkables.Length; i++)
                if (walkables[i])
                    count++;
            // ReSharper disable once PossibleLossOfFraction
            var intensity = (byte) Mathf.FloorToInt(255 / validCount * count);
            
            return new Color32(intensity, 0, 0, 255);
        }

        private void OnResize(Vector2 pixelSize)
        {
            var pixelSizeInt = new Vector2Int((int)pixelSize.x, (int)pixelSize.y);
            if (_completeRT != null)
                RenderTexture.ReleaseTemporary(_completeRT);
            _completeRT = RenderTexture.GetTemporary(pixelSizeInt.x, pixelSizeInt.y, 0, RenderTextureFormat.Default);
        }

        public void OnUpdateCommandBuffer(CommandBuffer cb)
        {
            cb.SetRenderTarget(_completeRT);
            cb.ClearRenderTarget(true, true, Color.clear);
            cb.Blit(_staticStaticTexture, _completeRT);
        }
    }
}