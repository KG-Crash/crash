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
        private Texture2D _staticTexture;
        private static readonly int StaticTexture = Shader.PropertyToID("_StaticTexture");

        public Texture2D texture => _staticTexture;

        public void LoadMapData(KG.Map map, int mergeCellSize, float scaler)
        {
            if (map.width % mergeCellSize > 0 || map.height % mergeCellSize > 0)
                throw new Exception($"map size not sqaured, ({map.width},{map.height})");

            var cellWidth = map.width / mergeCellSize;
            var cellHeight = map.height / mergeCellSize;

            var staticCells = new Color32[cellWidth * cellHeight];
            Span<bool> walkables = stackalloc bool[mergeCellSize * mergeCellSize];
            for (var cellYIndex = 0; cellYIndex < cellHeight; cellYIndex++)
            for (var cellXIndex = 0; cellXIndex < cellWidth; cellXIndex++)
            {
                for (var localYIndex = 0; localYIndex < mergeCellSize; localYIndex++)
                for (var localXIndex = 0; localXIndex < mergeCellSize; localXIndex++)
                {
                    var bufferIndex = localYIndex * mergeCellSize + localXIndex;
                    var mapIndex = ((cellYIndex * mergeCellSize + localYIndex) * map.width) +
                                   (cellXIndex * mergeCellSize + localXIndex);
                    
                    walkables[bufferIndex] = map._walkability[mapIndex];
                }

                staticCells[cellYIndex + (cellWidth - 1 - cellXIndex) * cellHeight] = ToColor32(walkables);
            }

            _staticTexture = new Texture2D(map.width / mergeCellSize, map.height / mergeCellSize, TextureFormat.RGBA32, false);
            _staticTexture.name = $"{nameof(Minimap)} Texture";
            _staticTexture.filterMode = FilterMode.Point;
            _staticTexture.SetPixels32(staticCells);
            _staticTexture.Apply();
        }

        private static Color32 ToColor32(Span<bool> walkables)
        {
            var count = 0;
            for (var i = 0; i < walkables.Length; i++)
                if (walkables[i])
                    count++;
            // ReSharper disable once PossibleLossOfFraction
            var intensity = (byte) Mathf.FloorToInt(255 / walkables.Length * count);
            
            return new Color32(intensity, 0, 0, 255);
        }
    }
}