using Game;
using UI;
using UnityEngine;

[System.Serializable]
public struct MinimapOption
{
    public float staticScaler;
    public int staticMergeCellSize;
}

public partial class GameState
{
    private Minimap _minimap;

    [SerializeField]
    private MinimapOption _minimapOptions = new MinimapOption { staticScaler = 1, staticMergeCellSize = 2 };
    
    private void InitializeGamePanel(KG.Map map)
    {
        var gamePanel = GetView<GamePanel>();
        gamePanel.upgradeOpenClick.AddListener(OnClickUpgrade);
        gamePanel.exitClick.AddListener(OnClickExit);
        gamePanel.attackTargetChange.AddListener(OnAttackTargetChange);
        gamePanel.gameDragEvent.AddListener(OnDragEvent);

        _minimap = new Minimap();
        _minimap.LoadMapData(map, _minimapOptions.staticMergeCellSize, _minimapOptions.staticScaler);
        gamePanel.Initialize(_minimap.staticTexture);
    }

    private void ReadyGamePanel(int playerCount)
    {
        GetView<GamePanel>().Ready(playerCount);
    }
    
    [UpdateLockStep]
    private void UpdateGamePanel(Frame input, Frame output) {}

    private void ClearGamePanel()
    {
        var gamePanel = GetView<GamePanel>();
        gamePanel.upgradeOpenClick.RemoveListener(OnClickUpgrade);
        gamePanel.exitClick.RemoveListener(OnClickExit);
        gamePanel.attackTargetChange.RemoveListener(OnAttackTargetChange);
        gamePanel.gameDragEvent.RemoveListener(OnDragEvent);
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