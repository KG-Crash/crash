using FixMath.NET;
using Game;
using Shared.Type;
using UI;

public partial class GameState
{
    private void InitializeDebugPanel()
    {
        var upgradePanel = GetView<GameDebugPanel>();
        upgradePanel.onClick1.AddListener(OnUnitSpawnClick1);
        upgradePanel.onClick2.AddListener(OnUnitSpawnClick2);
        upgradePanel.onClick3.AddListener(OnUnitSpawnClick3);
    }

    private void ClearDebugPanel()
    {
        var upgradePanel = GetView<GameDebugPanel>();
        upgradePanel.onClick1.RemoveListener(OnUnitSpawnClick1);
        upgradePanel.onClick2.RemoveListener(OnUnitSpawnClick2);
        upgradePanel.onClick3.RemoveListener(OnUnitSpawnClick3);
    }

    private void OnUnitSpawnClick1()
    {
        var pos = spawnPositions[this.me.id].position;
        EnqueueSpawn(Shared.Const.Debug.UnitSpawnType1, (uint)Shared.Const.Debug.UnitSpawnCount1, new FixVector2() { x = pos.x, y = pos.z });
    }

    private void OnUnitSpawnClick2()
    {
        var pos = spawnPositions[this.me.id].position;
        EnqueueSpawn(Shared.Const.Debug.UnitSpawnType2, (uint)Shared.Const.Debug.UnitSpawnCount2, new FixVector2() { x = pos.x, y = pos.z });
    }

    private void OnUnitSpawnClick3()
    {
        var pos = spawnPositions[this.me.id].position;
        EnqueueSpawn(Shared.Const.Debug.UnitSpawnType3, (uint)Shared.Const.Debug.UnitSpawnCount3, new FixVector2() { x = pos.x, y = pos.z });
    }
}