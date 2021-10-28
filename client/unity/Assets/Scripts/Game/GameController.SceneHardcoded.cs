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

        /*
         * 유닛 스폰 및 해당 맵의 첫번째 처리 루틴,
         * 지금은 임시 코드가 들어감.
         */
        private void OnLoadScene()
        {   
            _allPlayerByTeam = new Team();
            
            var player = AddNewPlayer(0, 0);
            
            SpawnUnitToPlayerStart(3, player);
            //SpawnUnitToPlayerStart(1, player);
            //SpawnUnitToPlayerStart(2, player);

            var otherPlayer = AddNewPlayer(1, 1);
            
            SpawnUnitToPlayerStart(0, otherPlayer);
            SpawnUnitToPlayerStart(1, otherPlayer);

            _player = player;

            if (_player.units.Any())
            {
                var sum = _player.units.Aggregate(FixVector3.Zero, (acc, x) => acc + x.position);
                var lookPosition = sum / _player.units.Count();
                _focusTransform.position = lookPosition;
            }
        }
    }
}