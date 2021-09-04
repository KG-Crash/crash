using Shared;
using UnityEngine;

namespace Game
{
    public partial class GameController : Player.IPlayerListener 
    {
        public void AttackTargetChanged(uint playerID, uint? targetPlayerID)
        {
            Debug.Log($"PlayerLevelChanged({playerID}, {targetPlayerID})");
        }

        public void PlayerLevelChanged(uint playerID, uint level)
        {
            Debug.Log($"PlayerLevelChanged({playerID}, {level})");
        }

        public void FinishUpgrade(Ability ability)
        {
            Debug.Log($"FinishUpgrade({ability})");
        }
    }
}