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
            public static readonly Fix64 _radiusinc = 1;
            public static readonly Fix64 _radianmax = Fix64.Pi * 2;
            public static readonly Fix64 _radianinc = _radianmax / (10);
            
            public Fix64 _radius = Fix64.Zero;
            public Fix64 _radian = _radianmax;
            public Fix64 _startRadian = Fix64.Zero;
            public List<Unit> _placedUnits = new List<Unit>();

            public FixVector3 GetOffset()
            {
                return new FixVector3(Fix64.Cos(_radian + _startRadian), 0, Fix64.Sin(_radian + _startRadian)) * _radius;
            }

            public void IncreaseOffset()
            {
                if (_radian >= _radianmax)
                {
                    _radius += _radiusinc;
                    _radian = Fix64.Zero;
                }
                else
                {
                    _radian += _radianinc;
                }
            }
            
            public static void PlaceUnit(KG.Map map, TemporalPlaceContext ctx, Unit unit, FixVector3 centerPosition)
            {
                var pos = centerPosition;
                
                while (true)
                {
                    pos = centerPosition + ctx.GetOffset();
                    var rect = unit.GetCollisionBox(pos);
                    var noneCollided = true;

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
                    
                    ctx.IncreaseOffset();
                }
                
                unit.position = pos;
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
            var newUnit = _unitFactory.CreateNewUnit(spawnUnitOriginID, _unitPrefabTable, _map, ownPlayer, this, _unitParent);
            ownPlayer.units.Add(newUnit);
            TemporalPlaceContext.PlaceUnit(_map, context, newUnit, GetSpawnPosition(ownPlayer.spawnIndex));

            return newUnit;
        }

        public Unit SpawnUnitToPosition(int spawnUnitOriginID, Player ownPlayer, FixVector3 centerPosition, TemporalPlaceContext context)
        {
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
            var placeContext = new TemporalPlaceContext();
            
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
            var ctx = new TemporalPlaceContext() { _startRadian = Fix64.Pi / 180.0f * rot.y };

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