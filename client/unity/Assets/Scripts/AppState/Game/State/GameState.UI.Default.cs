using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UI;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct MinimapOption
{
    public float staticScaler;
    public int staticMergeCellSize;

    public Vector2 unitPixelSize;
    public Color staticCellColor;
    public Color[] unitTeamColors;
}

public partial class GameState
{
    private Minimap _minimap;
    private CommandBuffer _updateMinimapCb;
    
    private List<Vector4> _unitNormalizedPositions = new List<Vector4>();
    private List<float> _unitColorIndices = new List<float>();
    private Material _unitRectMaterial;

    [SerializeField]
    private MinimapOption _minimapOptions = new MinimapOption
    {
        staticScaler = 1, staticMergeCellSize = 2, staticCellColor = Color.red, 
        unitPixelSize = new Vector2(25f,25f),
        unitTeamColors = new Color[] {Color.white, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan}
    };
    
    private void InitializeGamePanel(KG.Map map, Camera camera)
    {
        var gamePanel = GetView<GamePanel>();
        gamePanel.upgradeOpenClick.AddListener(OnClickUpgrade);
        gamePanel.exitClick.AddListener(OnClickExit);
        gamePanel.attackTargetChange.AddListener(OnAttackTargetChange);
        gamePanel.gameDragEvent.AddListener(OnDragEvent);

        _minimap = new Minimap();
        _minimap.LoadMapData(gamePanel.minimapViewSize, map, _minimapOptions.staticMergeCellSize, _minimapOptions.staticScaler, _minimapOptions.staticCellColor);
        gamePanel.Initialize(_minimap.minimapTexture);
        
        _updateMinimapCb = new CommandBuffer();
        _minimap.OnUpdateCommandBuffer(_updateMinimapCb);
        camera.AddCommandBuffer(CameraEvent.AfterSkybox, _updateMinimapCb);

        _unitRectMaterial = new Material(Shader.Find("Content/UnitToMinimap"));

        var maxCount = 500;
        _unitNormalizedPositions.Clear();
        _unitNormalizedPositions.AddRange(Enumerable.Repeat(Vector4.zero, maxCount * 4 / 2));
        _unitColorIndices.Clear();
        _unitColorIndices.AddRange(Enumerable.Repeat(0.0f, maxCount));
    }

    private void ReadyGamePanel(int playerCount)
    {
        GetView<GamePanel>().Ready(playerCount);
    }

    [UpdateLockStep]
    private void UpdateGamePanel(Frame input, Frame output)
    {
        _updateMinimapCb.Clear();
        _minimap.OnUpdateCommandBuffer(_updateMinimapCb);

        var map = UnityEngine.Object.FindObjectOfType<KG.Map>();
        
        // 유닛 위치 업데이트
        var minimapViewSize = GetView<GamePanel>().minimapViewSize;
        var normalizedUnitSize = _minimapOptions.unitPixelSize / minimapViewSize;
        var unitCount = 0;
        
        foreach (var unit in unitActorMaps.Keys.OfType<Unit>())
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
        }

        if (_unitNormalizedPositions.Count > 0)
        {
            _unitRectMaterial.SetVectorArray("_UnitPositions", _unitNormalizedPositions);
            _unitRectMaterial.SetFloatArray("_UnitColorIndices", _unitColorIndices);
            _unitRectMaterial.SetColorArray("_UnitColors", _minimapOptions.unitTeamColors);
            _updateMinimapCb.DrawProcedural(
                Matrix4x4.identity, _unitRectMaterial, 0, MeshTopology.Quads, 4, unitCount
            );
        }
    }

    private void ClearGamePanel()
    {
        var gamePanel = GetView<GamePanel>();
        gamePanel.upgradeOpenClick.RemoveListener(OnClickUpgrade);
        gamePanel.exitClick.RemoveListener(OnClickExit);
        gamePanel.attackTargetChange.RemoveListener(OnAttackTargetChange);
        gamePanel.gameDragEvent.RemoveListener(OnDragEvent);
        
        _unitNormalizedPositions.Clear();
        _unitColorIndices.Clear();
    }

    private void OnClickUpgrade()
    {
        ShowView<UpgradePanel>();
    }
    
    private void OnClickExit()
    {
        _ = MoveStateAsync<LobbyState>();
    }

    private void OnAttackTargetChange(int? attackTarget)
    {
        EnqueueAttackPlayer(attackTarget == null? 0: (uint) attackTarget.Value);
    }

    private void OnDragEvent(Vector2 delta)
    {
        var unityObject = UnityResources._instance.Get("objects");
        var focusTransform = unityObject.GetFocus();
        var lastDelta = delta * (float)Shared.Const.Input.DragDelta;
        focusTransform.position += new Vector3(lastDelta.y, 0, -lastDelta.x);
    }
}