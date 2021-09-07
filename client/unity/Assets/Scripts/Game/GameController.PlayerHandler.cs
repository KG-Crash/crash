using System;
using System.Linq;
using Shared;
using UnityEngine;

namespace Game
{
    public partial class GameController : Player.IPlayerListener 
    {
        void UpdateForDebug()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
                _player.StartUpgrade(Ability.UPGRADE_1);
            if (Input.GetKeyUp(KeyCode.Alpha2))
                _player.StartUpgrade(Ability.UPGRADE_2);
            if (Input.GetKeyUp(KeyCode.Alpha3))
                _player.StartUpgrade(Ability.UPGRADE_3);
            if (Input.GetKeyUp(KeyCode.Alpha4))
                _player.StartUpgrade(Ability.UPGRADE_4);
            if (Input.GetKeyUp(KeyCode.Alpha5))
                _player.StartUpgrade(Ability.UPGRADE_5);
            if (Input.GetKeyUp(KeyCode.Alpha6))
                _player.StartUpgrade(Ability.UPGRADE_6);
            if (Input.GetKeyUp(KeyCode.Alpha7))
                _player.StartUpgrade(Ability.UPGRADE_7);
            if (Input.GetKeyUp(KeyCode.Alpha8))
                _player.StartUpgrade(Ability.UPGRADE_8);
            if (Input.GetKeyUp(KeyCode.Alpha9))
                _player.StartUpgrade(Ability.UPGRADE_9);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _player.targetPlayerID = 1;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _player.targetPlayerID = null;
            }
        }
        
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
                var targetPosition = _map.GetSpawnPosition(targetPlayer.spawnIndex);
                player.units.OrderBy(x => x.unitID).FirstOrDefault().MoveTo(targetPosition);
                //foreach (var unit in player.units)
                //{
                //    unit.MoveTo(targetPosition);
                //}
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