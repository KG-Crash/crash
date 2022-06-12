using Shared;
using UnityEngine;

namespace Game
{
    public partial class GameController
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
}