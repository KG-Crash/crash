using System;
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
        if (player.id == Client.Instance.id)
            Bind(player);   
    }
    
    public void OnFinishUpgrade(Ability ability)
    {
        EnqueueUpgrade(ability);
    }

    public void OnAttackTargetChanged(Player player, Player target)
    {
        EnqueueAttackPlayer((uint)target.id);
    }

    public void OnPlayerLevelChanged(Player player, uint level)
    {
        Debug.Log($"OnPlayerLevelChanged({player.id}, {level})");
    }
}