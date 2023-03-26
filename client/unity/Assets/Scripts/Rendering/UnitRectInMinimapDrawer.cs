using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitRectInMinimapDrawer
{
    private List<Vector4> _unitNormalizedPositions = new List<Vector4>();
    private List<float> _unitColorIndices = new List<float>();
    private Material _unitRectMaterial;
    private readonly int _maxCount;

    public UnitRectInMinimapDrawer(int maxCount)
    {
        _maxCount = maxCount;
        _unitRectMaterial = new Material(Shader.Find("Content/UnitToMinimap"));
    }

    public void Initialize()
    {
        _unitNormalizedPositions.Clear();
        _unitNormalizedPositions.AddRange(Enumerable.Repeat(Vector4.zero, _maxCount * 4 / 2));
        _unitColorIndices.Clear();
        _unitColorIndices.AddRange(Enumerable.Repeat(0.0f, _maxCount));
    }
    
    public void DrawUnitRectsToMinimap(CommandBuffer minimapCb, KG.Map map, Color[] unitTeamColors, IEnumerable<Unit> units, Vector2 normalizedUnitSize)
    {
        // 유닛 위치 업데이트
        var unitCount = 0;

        void DrawUnitRects(int drawCount, List<Vector4> unitPositions, List<float> unitColorIndices)
        {
            minimapCb.SetGlobalVectorArray("_UnitPositions", unitPositions);
            minimapCb.SetGlobalFloatArray("_UnitColorIndices", unitColorIndices);
            minimapCb.DrawProcedural(
                Matrix4x4.identity, _unitRectMaterial, 0, MeshTopology.Quads, 4, drawCount
            );
        }
        
        _unitRectMaterial.SetColorArray("_UnitColors", unitTeamColors);

        foreach (var unit in units)
        {
            var p = new Vector2(
                unit.position.x / map.width,
                unit.position.z / map.height
            );
            _unitNormalizedPositions[unitCount * 2 + 0] = new Vector4(
                p.y - normalizedUnitSize.x, p.x - normalizedUnitSize.y,
                p.y - normalizedUnitSize.x, p.x + normalizedUnitSize.y
            );
            _unitNormalizedPositions[unitCount * 2 + 1] = new Vector4(
                p.y + normalizedUnitSize.x, p.x + normalizedUnitSize.y,
                p.y + normalizedUnitSize.x, p.x - normalizedUnitSize.y
            );

            _unitColorIndices[unitCount] = unit.team.id;

            unitCount++;

            if (unitCount + 1 >= _maxCount)
            {
                DrawUnitRects(unitCount, _unitNormalizedPositions, _unitColorIndices);
                unitCount = 0;
            }
        }

        if (unitCount > 0) DrawUnitRects(unitCount, _unitNormalizedPositions, _unitColorIndices);
    }

}