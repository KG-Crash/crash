using System;
using System.Collections.Generic;
using System.Linq;
using FixMath.NET;
using KG;
using UnityEngine;

namespace Game
{
    public partial class GameController
    {
        #region 유닛 스폰 필드
        
        [Header("Unit Spawn")] 
        [SerializeField] private Transform _unitParent;
        [SerializeField] private Transform[] _spawnPositions;

        #endregion 유닛 스폰 필드
        #region 유닛 스폰 로직
        public class TemporalPlaceContext
        {
            public List<Unit> _placedUnits = new List<Unit>();
            public Queue<Map.Cell> _nearCellQueue = new Queue<Map.Cell>();
            public uint threshold { get; private set; } = 10; // temp
            public bool isValid { get; private set; } = true;

            public static void PlaceUnit(KG.Map map, TemporalPlaceContext ctx, Unit unit, FixVector3 centerPosition)
            {
                FixVector3 pos;
                Map.Cell nowCell;

                if(ctx._nearCellQueue.Count <= 0)
                    ctx._nearCellQueue.Enqueue(map[centerPosition]);

                while (true)
                {
                    nowCell = ctx._nearCellQueue.Dequeue();
                    pos = nowCell.position;
                    var rect = unit.GetCollisionBox(pos);
                    var noneCollided = true;

                    if ((pos - centerPosition).magnitude >= (Fix64)ctx.threshold)
                    {
                        ctx.isValid = false;
                        ctx._placedUnits.Add(unit);

                        foreach (var placedUnit in ctx._placedUnits)
                        {
                            unit.owner.units.Delete(placedUnit);
                            Destroy(placedUnit.gameObject);
                        }
                        ctx._placedUnits.Clear();

                        return;
                    } 

                    foreach (var collideUnit in map.GetRegionUnits(rect))
                    {
                        if (collideUnit.collisionBox.Contains(rect))
                        {
                            if (collideUnit.collisionBox.Contains(rect))
                            {
                                noneCollided = false;
                                break;
                            }
                        }
                    }
                    if (noneCollided)
                        foreach(var placedUnit in Enumerable.Reverse(ctx._placedUnits))
                        {
                            if (placedUnit.collisionBox.Contains(rect))
                            {
                                noneCollided = false;
                                break;
                            }
                        }

                    if (noneCollided)
                        break;

                    foreach(var near in map.NearsForSpawn(nowCell))
                    {
                        if(!ctx._nearCellQueue.Contains(near))
                            ctx._nearCellQueue.Enqueue(near);
                    }
                }
                unit.SetPosition(pos, true);
                ctx._placedUnits.Add(unit);
            }
        }
        public FixVector3 GetSpawnPosition(int index)
        {
            return _spawnPositions[index].position;
        }
        public FixVector3 GetSpawnRotation(int index)
        {
            return _spawnPositions[index].rotation.eulerAngles;
        }
        
        public Unit SpawnUnitToPlayerStart(int spawnUnitOriginID, Player ownPlayer, TemporalPlaceContext context)
        {
            if (!context.isValid)
                return null;

            var newUnit = _unitFactory.CreateNewUnit(spawnUnitOriginID, _unitPrefabTable, _map, ownPlayer, this, _unitParent);
            ownPlayer.units.Add(newUnit);
            TemporalPlaceContext.PlaceUnit(_map, context, newUnit, GetSpawnPosition(ownPlayer.spawnIndex));

            return newUnit;
        }

        public Unit SpawnUnitToPosition(int spawnUnitOriginID, Player ownPlayer, FixVector3 centerPosition, TemporalPlaceContext context)
        {
            if (!context.isValid)
                return null;
            
            var newUnit = _unitFactory.CreateNewUnit(spawnUnitOriginID, _unitPrefabTable, _map, ownPlayer, this, _unitParent);
            ownPlayer.units.Add(newUnit);
            TemporalPlaceContext.PlaceUnit(_map, context, newUnit, centerPosition);

            return newUnit;
        }

        public Unit SpawnUnitToPosition(int spawnUnitOriginID, uint playerID, FixVector3 centerPosition, TemporalPlaceContext context)
        {
            return SpawnUnitToPosition(spawnUnitOriginID, GetPlayer(playerID), centerPosition, context);
        }

        #endregion 유닛 스폰 로직 
        
        #region 유닛 스폰 하드코드
        
        /*
         * 유닛 스폰 및 해당 맵의 첫번째 처리 루틴,
         * 지금은 임시 코드가 들어감.
         */
        private void OnLoadScene()
        {   
            _allPlayerByTeam = new Team();
            
            var player1 = AddPlayerAndUnit(true,  0, new[] {  0,  1,  2,  3,  3 });
            var player2 = AddPlayerAndUnit(true,  1, new[] {  0,  1,  2,  3,  3 });
            var player3 = AddPlayerAndUnit(true,  2, new[] {  0,  1,  2,  3,  3 });
            
            var player4 = AddPlayerAndUnit(false, 3, new[] {  0,  1,  2,  3,  3 });
            var player5 = AddPlayerAndUnit(false, 4, new[] {  0,  1,  2,  3,  3 });
            var player6 = AddPlayerAndUnit(false, 5, new[] {  0,  1,  2,  3,  3 });

            player1.targetPlayerID = player4.playerID;
            player2.targetPlayerID = player5.playerID;
            player3.targetPlayerID = player6.playerID;
            
            player4.targetPlayerID = player1.playerID;
            player5.targetPlayerID = player2.playerID;
            player6.targetPlayerID = player3.playerID;
                        
            MoveCameraToUnitPos(player1);
        }

        private Player AddPlayerAndUnit(bool home, int spawnIndex, params int[] unitTypes)
        {
            var player = AddNewPlayer(home? (uint)0: 1, spawnIndex);
            var rot = GetSpawnRotation(spawnIndex);
            var ctx = new TemporalPlaceContext();
            foreach (var unitType in unitTypes)
            {
                SpawnUnitToPlayerStart(unitType, player, ctx);
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