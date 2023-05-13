using System;
using System.Collections.Generic;
using System.Linq;
using FixMath.NET;
using Game;
using Network;
using Shared.Table;
using Shared.Type;
using UnityEditorInternal;
using UnityEngine;

public partial class GameState : Team.Listener
{
    public void OnSpawned(Player player)
    {
        if (player.id == id)
            Bind(player);   
    }
    
    public void OnFinishUpgrade(Ability ability)
    {
        EnqueueUpgradeFinish(ability);
    }

    public void OnUpgradeCancel(Ability ability)
    {
        EnqueueUpgradeCancel(ability);
    }

    public void OnSpawnMyUnitByUpgrade(UnitUpgradeSpawn spawn)
    {
        FixVector3 position = spawnPositions[id].position;
        var xzpos = new FixVector2(position.x, position.z);
        EnqueueSpawn(spawn.Unit, (uint)spawn.Count, xzpos);
    }
    
    public void OnAttackTargetChanged(Player player, Player target)
    {
        foreach (var unit in player.units.Values)
            UnitMoveToTarget(unit, target);
    }

    public void OnPlayerLevelChanged(Player player, uint level)
    {
        Debug.Log($"OnPlayerLevelChanged({player.id}, {level})");
    }
}