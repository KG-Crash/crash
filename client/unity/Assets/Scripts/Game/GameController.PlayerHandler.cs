using System;
using System.Linq;
using Shared;
using UnityEngine;

namespace Game
{
    public partial class GameController : Player.IPlayerListener 
    {
        public void OnAttackTargetChanged(uint playerID, uint? targetPlayerID)
        {
            var player = GetPlayer(playerID);

            if (targetPlayerID == null)
            {
                foreach (var unit in player.units)
                {
                    unit.Stop();
                }
            }
            else
            {
                var targetPlayer = GetPlayer(targetPlayerID.Value);
                var targetPosition = GetSpawnPosition(targetPlayer.spawnIndex);
                foreach (var unit in player.units)
                {
                    unit.MoveTo(targetPosition);
                }
            }
        }

        public void OnPlayerLevelChanged(uint playerID, uint level)
        {
            Debug.Log($"OnPlayerLevelChanged({playerID}, {level})");
        }

        public void OnFinishUpgrade(Ability ability)
        {
            Debug.Log($"OnFinishUpgrade({ability})");
        }
    }
}