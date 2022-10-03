using Game;
using Network;
using Shared.Type;
using UnityEngine;

public partial class GameState : Team.Listener
{
    public void OnSpawned(Player player)
    {
        if (player.id == Client.Instance.id)
            Bind(player.upgrade);   
    }

    // TODO :: 나가는거 처리 넣으면 내용 추가
    public void OnLeave(Player player) { }
    
    public void OnFinishUpgrade(Ability ability)
    {
        Debug.Log($"OnFinishUpgrade({ability})");
        
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