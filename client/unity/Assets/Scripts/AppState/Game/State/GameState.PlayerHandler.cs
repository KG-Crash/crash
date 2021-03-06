using Shared;
using UnityEngine;
using Game;

public partial class GameState : Team.Listener
{
    public void OnFinishUpgrade(Ability ability)
    {
        Debug.Log($"OnFinishUpgrade({ability})");
    }

    public void OnAttackTargetChanged(Player player, Player target)
    {
    }

    public void OnPlayerLevelChanged(Player player, uint level)
    {
        Debug.Log($"OnPlayerLevelChanged({player.id}, {level})");
    }
}