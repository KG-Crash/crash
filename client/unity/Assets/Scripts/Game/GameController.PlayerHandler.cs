using Shared;
using UnityEngine;

namespace Game
{
    public partial class GameController : Player.IPlayerListener 
    {
        public void AttackTargetChanged(int playerID, int targetPlayerID)
        {
            Debug.Log($"PlayerLevelChanged({playerID}, {targetPlayerID})");
        }

        public void PlayerLevelChanged(int playerID, int level)
        {
            Debug.Log($"PlayerLevelChanged({playerID}, {level})");
        }

        public void FinishUpgrade(Ability ability)
        {
            Debug.Log($"FinishUpgrade({ability})");
        }
    }
}