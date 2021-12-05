using System;
using System.Collections.Generic;
using System.Linq;
using FixMath.NET;
using KG;
using Shared;
using UnityEngine;

namespace Game
{
    public partial class Unit
    {
        [Header("경로에 상관없이 원하는 목적지")]
        [NonSerialized] private Stack<FixVector3> _destStack = new Stack<FixVector3>();
        [NonSerialized] private float _stopMoveDistance = 0.0f;
        
        [Header("경로 데이터")]
        [NonSerialized] private List<KG.Map.Region> _regionPath = new List<KG.Map.Region>();
        [NonSerialized] private List<KG.Map.Cell> _cellPath = new List<KG.Map.Cell>();
        
        [Header("다른 유닛과 부딪힌 횟수")]
        [NonSerialized] private int _blocked;
        [NonSerialized] private DateTime? _blockedTime;

        public bool remainPath => _regionPath.Count > 0 || _destStack.Count > 0;
        public bool remainImmediatePath => _cellPath.Count > 0;

        private void DeltaMove(Fix64 delta)
        {
            if (_blockedTime != null && (DateTime.Now - _blockedTime.Value).TotalSeconds < 1)
                return;

            var dst = _cellPath.FirstOrDefault();
            if (dst == null)
            {
                return;
            }

            var diff = new FixVector3(dst.position.x - position.x, Fix64.Zero, dst.position.y - position.z);
            var magnitude = diff.magnitude;
            var direction = FixVector3.Zero;
            bool arrived;
            if (magnitude == Fix64.Zero)
            {
                arrived = true;
            }
            else
            {
                direction = diff / magnitude;

                // TODO : 이거는 나중에 동기화 때 처리해야 할 문제 (Time.deltaTime을 사용하지 않아야 함)
                transform.LookAt(new FixVector3(dst.position.x, this.transform.position.y, dst.position.y));

                arrived = magnitude < (speed * delta) || magnitude < (Fix64)_stopMoveDistance + (Fix64)Shared.Const.Character.MoveEpsilon;
            }

            if (arrived)
            {
                _cellPath.Remove(dst);
                if (_cellPath.Count == 0)
                {
                    if (_regionPath.Count == 0)
                    {
                        if (_destStack.Count > 0)
                        {
                            _destStack.Pop();  
                        
                            if (_destStack.Count > 0)
                            {
                                UpdateMovePath(_destStack.Peek(), true);
                            }
                        }
                    }
                    else
                    {
                        UpdateMovePath(_destStack.Peek());
                    }
                }
            }
            else
            {
                var old = new FixVector3(position);
                position += (direction * speed * delta);
                var collisionUnit = GetNearUnits().FirstOrDefault(x => !x.IsDead && x.collisionBox.Contains(this.collisionBox));
                if (collisionUnit != null)
                {
                    _blocked++;
                    if (_blocked > 2)
                    {
                        UpdateMovePath(_destStack.Peek(), units: new List<Unit> { collisionUnit });
                        _blocked = 0;
                        _blockedTime = null;
                    }
                    else
                    {
                        _blockedTime = DateTime.Now;
                    }

                    position = old;
                }
                else
                {
                    _blocked = 0;
                    _blockedTime = null;
                }
            }
        }

        private List<KG.Map.Region> GetAllowedRegions(KG.Map.Region start, KG.Map.Region end)
        {
            if (start == end)
                return new List<KG.Map.Region> { start };

            if (_map.regions[start].edges.Contains(_map.regions[end]) == false)
                return new List<KG.Map.Region> { };

            var src = new[] { start, end };
            return src.Select(x => _map.regions[x]).SelectMany(x => x.edges)
                .Where(x => x.edges.Contains(_map.regions[start]) && x.edges.Contains(_map.regions[end]))
                .Select(x => x.data).Concat(src).Distinct().ToList();
        }

        private static Map.Cell WalkableCell(Unit unit, Map.Region region)
        {
            return region.centroid.Near(cell => unit.IsWalkable(cell) && cell.region == region);
        }
        private static Map.Cell WalkableCell(Unit unit, Map.Cell centerCell)
        {
            return centerCell.Near(cell => unit.IsWalkable(cell));
        }
        
        private bool UpdateMovePath(FixVector3 position, bool updateWithRegion = false, List<Unit> units = null)
        {
            try
            {
                var start = _map[this.position];
                if (start == null)
                    throw new ClientException(ClientExceptionCode.InvalidCellAccess, $"start, _map[{position}] == null");

                var end = _map[position];
                if (end == null)
                    throw new ClientException(ClientExceptionCode.InvalidCellAccess, $"end, _map[{position}] == null");

                if (this.table.Flyable)
                {
                    _cellPath = new List<KG.Map.Cell> { end };
                    return true;
                }

                if (updateWithRegion)
                    _regionPath = _map.regions.Find(this.region, end.region);

                var next = _regionPath.Count < 2 ? WalkableCell(this, end) : WalkableCell(this, _regionPath.First());
                if (!IsWalkable(next))
                    throw new ClientException(ClientExceptionCode.NotWalkableNextCell, $"next({next}) cell of region is not walkable, {_regionPath.Count}");

                var allowed = GetAllowedRegions(start.region, next.region);
                var collisionList = this.collisionCells;

                // 정지 상태인 유닛들도 충돌 조건에 포함한다.
                // 이동중인 유닛은 포함하지 않음
                // 공격중인 유닛은 어케 처리해야할지...
                var stopUnits = GetNearUnits().Where(x => x._currentState == UnitState.Idle);
                if (units != null)
                    stopUnits = stopUnits.Concat(units).Distinct();

                var unitCollideBoxes = stopUnits.Select(x => x.collisionBox).ToList();
                if (unitCollideBoxes.Any(x => x.Contains(end.collisionBox)))
                {
                    end = end.Near(x =>
                    {
                        if (IsWalkable(x) == false)
                            return false;

                        if (unitCollideBoxes.Any(y => y.Contains(GetCollisionBox(x.center, _map.cellSize))))
                            return false;

                        return true;
                    }, Fix64.One * 3);

                    if(end == null)
                        return false;

                    UpdateMovePath(end.center, true, units);
                    return true;
                }

                _cellPath = _map.cells.Find(start, next, node => 
                {
                    if (allowed.Any(x => x == node.data.region) == false)
                        return false;

                    if (IsWalkable(node.data) == false)
                        return false;
                    
                    if (unitCollideBoxes.Any(x => x.Contains(GetCollisionBox(node.data.center, _map.cellSize))))
                        return false;

                    return true;
                });
                
                if (_cellPath.Count == 0)
                    throw new ClientException(ClientExceptionCode.ZeroCellPath, "_cellPath.Count == 0");

                Debug.Log(
                    $"update detail route. unitID: {unitID}, _cellPath.Count: {_cellPath.Count}, _regionPath.Count: {_regionPath.Count}");
                return true;
            }
            catch (ClientException e)
            {
                Debug.LogError($"code: {e._code}, exception: {e.Message}, unitID: {unitID}");
                _cellPath.Clear();
                return false;
            }
        }
        
        public bool ResetMovePathAndDestination(FixVector3 position)
        {
            ClearPathAndDestination();

            _destStack.Push(position);
            return UpdateMovePath(position, true);
        }
        
        public void ClearPathAndDestination()
        {
            _destStack.Clear();
            _cellPath.Clear();
            _regionPath.Clear();
        }

        private bool TryUpdateMovePath()
        {
            if (_blockedTime != null && (DateTime.Now - _blockedTime.Value).TotalSeconds < 1)
                return false;
            if (_destStack.Count == 0)
                return false;
            
            _blocked++;
            if (_blocked > 2)
            {
                _blocked = 0;
                _blockedTime = null;
                
                if (UpdateMovePath(_destStack.Peek(), true))
                    return true;
            }
            else
            {
                _blockedTime = DateTime.Now;
            }
            
            return false;
        }
    }
}