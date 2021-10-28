using System.Linq;
using FixMath.NET;
using UnityEngine;

namespace Game
{
    public partial class GameController
    {
        public Unit SpawnUnitToPlayerStart(int spawnUnitOriginID, Player ownPlayer)
        {
            var newUnit = _unitFactory.GetNewUnit(spawnUnitOriginID, _unitPrefabTable, _map, ownPlayer, this);
            ownPlayer.units.Add(newUnit);
            newUnit.position = GetSpawnPosition(ownPlayer.spawnIndex);

            return newUnit;
        }

        public Unit SpawnUnitToPosition(int spawnUnitOriginID, Player ownPlayer, FixVector3 position)
        {
            var newUnit = _unitFactory.GetNewUnit(spawnUnitOriginID, _unitPrefabTable, _map, ownPlayer, this);
            ownPlayer.units.Add(newUnit);
            newUnit.position = position;

            return newUnit;
        }

        public Unit SpawnUnitToPosition(int spawnUnitOriginID, uint playerID, FixVector3 position)
        {
            return SpawnUnitToPosition(spawnUnitOriginID, GetPlayer(playerID), position);
        }
        
        #region 유닛 스폰 하드코드
        
        /*
         * 유닛 스폰 및 해당 맵의 첫번째 처리 루틴,
         * 지금은 임시 코드가 들어감.
         */
        private void OnLoadScene()
        {   
            _allPlayerByTeam = new Team();
            
            var player1 = AddPlayerAndUnit(true, 0, 3, 1, 2);
            var player2 = AddPlayerAndUnit(true, 1, 3, 1, 2);
            var player3 = AddPlayerAndUnit(true, 2, 3, 1, 2);
            
            var player4 = AddPlayerAndUnit(false, 3, 3, 1, 2);
            var player5 = AddPlayerAndUnit(false, 4, 3, 1, 2);
            var player6 = AddPlayerAndUnit(false, 5, 3, 1, 2);

            player1.targetPlayerID = player4.playerID;
            player2.targetPlayerID = player5.playerID;
            player3.targetPlayerID = player6.playerID;
            
            player4.targetPlayerID = player1.playerID;
            player5.targetPlayerID = player2.playerID;
            player6.targetPlayerID = player3.playerID;
                        
            MoveCameraToUnitPos(player1);
        }

        private Player AddPlayerAndUnit(bool home, int playerID, params int[] unitTypes)
        {
            var player = AddNewPlayer(home? (uint)0: 1, playerID);

            foreach (var unitType in unitTypes)
            {
                SpawnUnitToPlayerStart(unitType, player);
            }
            
            return player;
        }

        private void MoveCameraToUnitPos(Player player)
        {
            _player = player;

            if (_player.units.Any())
            {
                var sum = _player.units.Aggregate(FixVector3.Zero, (acc, x) => acc + x.position);
                var lookPosition = sum / _player.units.Count();
                _focusTransform.position = lookPosition;
            }
        }
        
        #endregion 유닛 스폰 하드코드
    }
}