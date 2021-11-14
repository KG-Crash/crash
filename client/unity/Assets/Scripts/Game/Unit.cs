using FixMath.NET;
using Shared;
using Shared.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KG;
using UnityEngine;

namespace Game
{
    public interface ISelectable
    {
        bool selectable { get; set; }
        Bounds bounds { get; }
    }

    public interface IRenderable
    {
        Renderer[] renderers { get; }
    }

    public class Unit : MonoBehaviour, ISelectable, IRenderable
    {
        public interface Listener
        {
            void OnAttack(Unit me, Unit you, Fix64 damage);
            void OnHeal(Unit me, Unit you, Fix64 heal);
            void OnDamaged(Unit me, Unit you, Fix64 damage);
            void OnDead(Unit unit, Unit from);
            void OnOwnerChanged(Player before, Player after, Unit unit);
            void OnStartMove(Unit unit);
            void OnEndMove(Unit unit);
            void OnClear(Unit unit);
            void OnFireProjectile(Unit me, Unit you, int projectileOriginID);
            void OnIdle(Unit unit);
        }

        public Shared.Table.Unit table => Table.From<TableUnit>()[this._unitOriginID];
        public List<Shared.Table.Skill> skills => Table.From<TableSkill>().Values.Where(x => x.Unit == this.unitOriginID).ToList();

        [SerializeField] private int _unitOriginID;
        [SerializeField] private GameObject _highlighted;
        [SerializeField] public Animator animator;

        public GameObject highlighted
        {
            get => _highlighted;
            set 
            {
                _highlighted = value;
                _highlighted.transform.SetParent(transform);
            }
        }
        
        public uint playerID
        {
            get => owner.playerID;
        }
        public uint teamID
        {
            get => owner.teamID;
        }

        public bool selectable
        {
            get => _selectable;
            set => _selectable = value;
        }

        public int unitOriginID
        {
            get => _unitOriginID;
            set => _unitOriginID = value;
        }

        public uint unitID
        {
            get => _unitID;
            set => _unitID = value;
        }

        public int damage
        {
            get
            {
                var damage = table.Damage;
                if (owner.advanced.TryGetValue(Advanced.UPGRADE_WEAPON, out var advanced))
                    damage += advanced;

                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.Damage, out var x))
                    damage += x;

                return damage;
            }
        }

        public int armor
        {
            get
            {
                var armor = table.Armor;
                if (owner.advanced.TryGetValue(Advanced.UPGRADE_ARMOR, out var advanced))
                    armor += advanced;

                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.Armor, out var x))
                    armor += x;

                return armor;
            }
        }

        public Fix64 speed
        {
            get
            {
                var speed = table.Speed;
                if (owner.advanced.TryGetValue(Advanced.UPGRADE_SPEED, out var advanced))
                    speed += advanced;

                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.Speed, out var x))
                    speed += x;

                return (Fix64)speed;
            }
        }

        [SerializeField] public float _speed = 1.0f;
        public int attackSpeed
        {
            get
            {
                var attackSpeed = table.AttackSpeed;

                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.AttackSpeed, out var x))
                    attackSpeed += x;

                return attackSpeed;
            }
        }

        public int attackRange
        {
            get
            {
                var attackRange = table.AttackRange;

                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.AttackRange, out var x))
                    attackRange += x;

                return attackRange;
            }
        }

        public AttackType attackType
        {
            get => table.AttackType;
        }

        public int projectileId
        {
            get => table.ProjectileID;
        }


        public Fix64 maxhp
        {
            get
            {
                var hp = table.Hp;
                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.Hp, out var x))
                    hp += x;

                return new Fix64(hp);
            }
        }

        public Fix64 visibleRange
        {
            get => table.VisibleRange;
        }

        public Fix64 hp
        {
            get => _hp;
        }

        public int killScore
        {
            get => table.KillScore;
        }

        public Listener listener
        {
            get => _listener;
            set => _listener = value;
        }

        public Player owner
        {
            get => _owner;
            set
            {
                var before = this._owner;
                _owner = value;
                _listener?.OnOwnerChanged(before, _owner, this);
            }
        }

        public KG.Map map
        {
            get => _map;
            set => _map = value;
        }

        private KG.Map.Region _region;
        public KG.Map.Region region
        {
            get => _region;
            set
            {
                if (_region == value)
                    return;

                _region?.units.Remove(this);
                value?.units.Add(this);
                _region = value;
            }
        }

        public List<Skill> activeSkills => skills.Where(x => owner.abilities.HasFlag(x.Condition)).ToList();

        [SerializeField] private uint _teamID;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private uint _unitID;
        [SerializeField] private DateTime _lastAttackTime = DateTime.MinValue;
        [SerializeField] private Fix64 _hp;
        [SerializeField] private Player _owner;
        [NonSerialized] private KG.Map _map;
        [NonSerialized] private Listener _listener;

        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }

        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [SerializeField] private Renderer[] _rendereres;
        [SerializeField] private Material[] _deadMaterials;
        [SerializeField] private int _maxAttackAnimCount;

        public int maxAttackAnimCount
        {
            get => _maxAttackAnimCount;
            set => _maxAttackAnimCount = value;
        }

        public Material[] deadMaterials
        {
            get => _deadMaterials;
            set => _deadMaterials = value;
        }
        
        [NonSerialized] private UnitState _currentState;
        
        [NonSerialized] private float _stopMoveDistance;
        [NonSerialized] private List<KG.Map.Region> _regionPath = new List<KG.Map.Region>();
        [NonSerialized] private List<KG.Map.Cell> _cellPath = new List<KG.Map.Cell>();
        [NonSerialized] private FixVector3? _destination;
        [NonSerialized] private Unit _target;
        [NonSerialized] private HashSet<Unit> _attackers = new HashSet<Unit>();

        public void AddAttacker(Unit unit)
        {
            _attackers.Add(unit);
        }

        // 다른 유닛과 부딪힌 횟수
        [NonSerialized] private int _blocked;
        [NonSerialized] private DateTime? _blockedTime;


        private FixVector3 _position;
        public FixVector3 position
        {
            get => _position;
            set
            {
                _position = value;
                this.region = this.map[this.position]?.region;

                // 길찾기 경로에서 현재 경로 제거
                this._regionPath.Remove(this.region);
            }
        }

        public FixVector2 size => new FixVector2(new Fix64(this.table.Width) / new Fix64(10000), new Fix64(this.table.Height) / new Fix64(10000));

        public FixRect collisionBox => GetCollisionBox(this.position);
        public IEnumerable<KG.Map.Cell> collisionCells => GetCollisionCells(this.position);

        public bool IsDead => _currentState == UnitState.Dead || _hp == Fix64.Zero;

        public bool IsNullOrDead(Unit unit)
        {
            return unit == null || unit.IsDead;
        }
        
        public void Awake()
        {
            animator.SetInteger("MaxAttack", _maxAttackAnimCount);
        }

        [ContextMenu("Gather renderers")]
        public void OnRefreshRenderers()
        {
            _rendereres = GetComponentsInChildren<Renderer>();
        }

        public FixRect GetCollisionBox(FixVector2 position)
        {
            var size = this.size;
            var half = size / new Fix64(2);
            return new FixRect(position.x - half.x, position.y - half.y, size.x, size.y);
        }

        public FixRect GetCollisionBox(FixVector2 position, Fix64 padding)
        {
            var collisionBox = GetCollisionBox(position);
            if (padding == Fix64.Zero)
                return collisionBox;

            return collisionBox.Padding(padding);
        }

        private IEnumerable<KG.Map.Cell> GetCollisionCells(FixVector2 position)
        {
            var collisionBox = GetCollisionBox(position);
            var min = new FixVector2(collisionBox.minX, collisionBox.minY);
            var max = new FixVector2(collisionBox.maxX, collisionBox.maxY);

            var cells = new List<KG.Map.Cell>();
            var begin = this.map[min];
            if (begin == null)
                yield break;

            var end = this.map[max];
            if (end == null)
                yield break;

            for (int row = begin.row; row <= end.row; row++)
            {
                for (int col = begin.col; col <= end.col; col++)
                {
                    yield return this.map[row, col];
                }
            }

            yield break;
        }

        public bool IsWalkable(KG.Map.Cell cell)
        {
            return GetCollisionCells(cell.position).All(x => x.walkable);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < _cellPath.Count - 1; i++)
            {
                var begin = _cellPath[i];
                var end = _cellPath[i + 1];
                Gizmos.DrawLine(begin.position, end.position);
            }

            //var area = this.collisionBox;
            //Gizmos.DrawLine(new Vector3(area.minX, 0, area.minY), new Vector3(area.maxX, 0, area.minY));
            //Gizmos.DrawLine(new Vector3(area.minX, 0, area.maxY), new Vector3(area.maxX, 0, area.maxY));
            //Gizmos.DrawLine(new Vector3(area.minX, 0, area.minY), new Vector3(area.minX, 0, area.maxY));
            //Gizmos.DrawLine(new Vector3(area.maxX, 0, area.minY), new Vector3(area.maxX, 0, area.maxY));

            //Gizmos.color = Color.red;
            var cells = this.collisionCells;
            foreach (var cell in cells)
            {
                var cellBox = new FixRect(cell.position.x, cell.position.y, cell.size, cell.size);

                Gizmos.DrawLine(new Vector3(cellBox.minX, 0, cellBox.minY), new Vector3(cellBox.maxX, 0, cellBox.minY));
                Gizmos.DrawLine(new Vector3(cellBox.minX, 0, cellBox.maxY), new Vector3(cellBox.maxX, 0, cellBox.maxY));
                Gizmos.DrawLine(new Vector3(cellBox.minX, 0, cellBox.minY), new Vector3(cellBox.minX, 0, cellBox.maxY));
                Gizmos.DrawLine(new Vector3(cellBox.maxX, 0, cellBox.minY), new Vector3(cellBox.maxX, 0, cellBox.maxY));
            }
            
#if UNITY_EDITOR

            var targetStr = IsNullOrDead(_target) ? _target == null? "null": $"dead({_target.unitID})" : _target.unitID.ToString();
            var destStr = _cellPath.Count > 0
                ? $"{_cellPath.Find(x => true).center.ToString("0.##", true)} -> {_cellPath.FindLast(x => true).center.ToString("0.##", true)}"
                : "none";

            UnityEditor.Handles.Label(transform.position, $"{unitID},{hp},{_currentState},{targetStr},{destStr}");
#endif
        }

        public void Init(uint unitID, KG.Map map, Player owner, Unit.Listener listener)
        {
            this.unitID = unitID;
            this._map = map;
            this._owner = owner;
            this._listener = listener;
            _hp = maxhp;
        }

        private void Update()
        {
            _totalBounds = new Bounds();
            foreach (var renderer in _rendereres)
            {
                if (_totalBounds == new Bounds())
                {
                    _totalBounds = renderer.bounds;
                }
                else
                {
                    _totalBounds.Encapsulate(renderer.bounds);   
                }
            }

            Action();

            this.transform.position = this.position;
        }

        private void Action()
        {
            switch (_currentState)
            {
                case UnitState.Idle:
                    if (IsNullOrDead(_target))
                    {
                        // 공격할 상대가 없다면, 공격자 중 시야 거리 -> 시야 거리에 있는 적 탐색
                        _target = _attackers
                            .Where(x => !IsNullOrDead(x) && ContainsVisibleRange(x.position))
                            .OrderBy(x => (x.position - position).sqrMagnitude)
                            .FirstOrDefault();
                        
                        if (IsNullOrDead(_target))
                            _target = SearchEnemyIn(visibleRange);

                        if (_target)
                        {
                            UpdateMovePath(_target.position, true);   
                        }
                    }
                    
                    if (!IsNullOrDead(_target))
                    {
                        if (ContainsAttackRange(_target.position))
                        {
                            _currentState = UnitState.Attack;
                            goto case UnitState.Attack;
                        }
                        else
                        {
                            _currentState = UnitState.Move;
                            _listener?.OnStartMove(this);
                            goto case UnitState.Move;
                        }
                    }
                    break;

                /*
                 * 이동 중에는 공격 범위에 적이 있으면 공격, 아니면 그대로 고
                 */
                case UnitState.Move:
                {
                    if (IsNullOrDead(_target))
                    {
                        // 공격할 상대가 없다면, 공격자 중 사거리 이내 -> 나머지 사거리 이내에 있는 적 탐색
                        _target = _attackers
                            .Where(x => !IsNullOrDead(x) && ContainsAttackRange(x.position))
                            .OrderBy(x => (x.position - position).sqrMagnitude)
                            .FirstOrDefault();
                        
                        if (IsNullOrDead(_target))
                            _target = SearchEnemyIn(attackRange);
                    }
                    else if (!ContainsAttackRange(_target.position))
                    {
                        // 공격할 상대가 사거리에 안닿으면, 공격자 중 사거리 이내 -> 나머지 사거리 이내에 있는 적 탐색
                        var inRangeUnit = _attackers
                            .Where(x => !IsNullOrDead(x) && ContainsAttackRange(x.position))
                            .OrderBy(x => (x.position - position).sqrMagnitude)
                            .FirstOrDefault();

                        if (IsNullOrDead(inRangeUnit))
                            inRangeUnit = SearchEnemyIn(attackRange);
                        
                        if (inRangeUnit != null)
                        {
                            // 바로 공격 때리기
                            _target = inRangeUnit;
                        }
                    }
                    
                    // 공격 가능?
                    if (!IsNullOrDead(_target) && ContainsAttackRange(_target.position))
                    {
                        _currentState = UnitState.Attack;
                        _listener?.OnEndMove(this);
                        goto case UnitState.Attack;
                    }
                    else if (_cellPath.Count == 0)
                    {
                        _currentState = UnitState.Idle;
                        _listener?.OnEndMove(this);
                        _listener?.OnIdle(this);
                        break;
                    }
                    else
                    {   
                        DeltaMove(GameController.TimeDelta);
                        break;
                    }
                }

                case UnitState.Attack:
                    if (IsNullOrDead(_target))
                    {
                        // 공격할 상대가 없다면, 공격자 중 사거리 이내 -> 나머지 사거리 이내에 있는 적 탐색
                        _target = _attackers
                            .Where(x => !IsNullOrDead(x) && ContainsAttackRange(x.position))
                            .OrderBy(x => (x.position - position).sqrMagnitude)
                            .FirstOrDefault();
                        
                        if (IsNullOrDead(_target))
                            _target = SearchEnemyIn(attackRange);
                    }
                    
                    if (IsNullOrDead(_target))
                    {
                        if (_cellPath.Count == 0)
                        {
                            _currentState = UnitState.Idle;
                            _listener?.OnIdle(this);
                        }
                        else
                        {
                            _currentState = UnitState.Move;
                            _listener?.OnStartMove(this);
                        }
                    }
                    else if (!ContainsAttackRange(_target.position))
                    {
                        if (_cellPath.Count == 0)
                        {
                            UpdateMovePath(_target.position, true);
                        }

                        _currentState = UnitState.Move;
                        _listener?.OnStartMove(this);
                    }
                    else
                    {
                        Attack(_target);   
                    }
                    break;
            }
        }

        private void DeltaMove(Fix64 delta)
        {
            if (_blockedTime != null && (DateTime.Now - _blockedTime.Value).TotalSeconds < 1)
                return;

            var dst = _cellPath.FirstOrDefault();
            if (dst == null)
            {
                Stop();
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
                        Stop();
                    else
                        UpdateMovePath(this._destination.Value);
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
                        UpdateMovePath(this._destination.Value, units: new List<Unit> { collisionUnit });
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

        private Unit SearchEnemyIn(Fix64 searchRadius)
        {
            return GetNearUnitsIn(searchRadius)
                .Where(x => !IsNullOrDead(x) && x.teamID != this.teamID)
                .OrderBy(x => (x.position - position).sqrMagnitude)
                .FirstOrDefault(x => ContainsRange(x.position, searchRadius));
        }

        public void Stop()
        {
            _currentState = UnitState.Idle;
            _destination = null;
            listener?.OnEndMove(this);

            UnityEngine.Debug.Log("stop moving");
        }

        public void Selected(bool select)
        {
            if (_highlighted) _highlighted.SetActive(select);
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
        
        private void UpdateMovePath(FixVector3 position, bool updateWithRegion = false, List<Unit> units = null)
        {
            try
            {
                var start = _map[this.position];
                if (start == null)
                    throw new Exception("start = _map[this.position] == null");

                var end = _map[position];
                if (end == null)
                    throw new Exception("end = _map[position] == null");

                if (this.table.Flyable)
                {
                    _cellPath = new List<KG.Map.Cell> { end };
                    return;
                }

                if (updateWithRegion)
                    _regionPath = _map.regions.Find(this.region, end.region);

                var next = _regionPath.Count < 2 ? WalkableCell(this, end) : WalkableCell(this, _regionPath.First());
                if (!IsWalkable(next))
                    throw new Exception($"next({next}) is not walkable, {_regionPath.Count}");

                var allowed = GetAllowedRegions(start.region, next.region);
                var collisionList = this.collisionCells;

                // 정지 상태인 유닛들도 충돌 조건에 포함한다.
                // 이동중인 유닛은 포함하지 않음
                // 공격중인 유닛은 어케 처리해야할지...
                var stopUnits = GetNearUnits().Where(x => x._currentState == UnitState.Idle);
                if (units != null)
                    stopUnits.Concat(units).Distinct();

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
                        return;

                    UpdateMovePath(end.center, true, units);
                    return;
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
                    throw new Exception("_cellPath.Count == 0");
                
                UnityEngine.Debug.Log($"update detail route. unitID: {unitID}, _cellPath.Count: {_cellPath.Count}, _regionPath.Count: {_regionPath.Count}");

                _destination = position;
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError($"exception: {e.Message}, unitID: {unitID}");
                _cellPath.Clear();
            }
        }

        public void MoveTo(FixVector3 position)
        {
            UpdateMovePath(position, true);

            _currentState = UnitState.Move;
            _target = null;
            _destination = position;
            _stopMoveDistance = 0.0f;

            listener?.OnStartMove(this);
        }

        public bool ContainsAttackRange(FixVector3 targetPosition)
        {
            var attackRange = this.attackRange;
            return (position - targetPosition).sqrMagnitude < attackRange * attackRange;
        }
        public bool ContainsVisibleRange(FixVector3 targetPosition)
        {
            var visibleRange = this.visibleRange;
            return (position - targetPosition).sqrMagnitude < visibleRange * visibleRange;
        }
        public bool ContainsRange(FixVector3 targetPosition, Fix64 range)
        {
            return (position - targetPosition).sqrMagnitude < range * range;
        }

        private Fix64 CalculateDamage(Unit unit)
        {
            var result = (Fix64)(damage - unit.armor);

            if (table.Type == UnitType.Explosive)
            {
                switch (unit.table.Size)
                {
                    case UnitSize.Small:
                        return result;

                    case UnitSize.Medium:
                        return result * (Fix64)0.75;

                    case UnitSize.Large:
                        return result * (Fix64)0.5;
                }
            }

            if (table.Type == UnitType.Concussive)
            {
                switch (unit.table.Size)
                {
                    case UnitSize.Large:
                        return result;

                    case UnitSize.Medium:
                        return result * (Fix64)0.75;

                    case UnitSize.Small:
                        return result * (Fix64)0.5;
                }
            }

            return result;
        }

        public void AddHP(Fix64 value, Unit from = null)
        {
            _hp += value;
            if (_hp > maxhp)
                _hp = maxhp;

            if (_hp < Fix64.Zero)
                _hp = Fix64.Zero;

            if (value < Fix64.Zero)
                listener?.OnDamaged(this, from, -value);
            else
                listener?.OnHeal(this, from, value);

            if (_hp <= Fix64.Zero)
                Die(from);
        }

        public bool Attack(Unit unit)
        {
            var now = DateTime.Now;
            if ((now - _lastAttackTime).TotalMilliseconds < attackSpeed)
                return false;

            var damage = CalculateDamage(unit);

            if (this.attackType == AttackType.Immediately)
            {
                unit.AddHP(-damage, this);
                listener?.OnAttack(this, unit, damage);
            }
            else
                listener?.OnFireProjectile(this, unit, this.projectileId);

            
            _lastAttackTime = DateTime.Now;

            _attackers.Clear();
            transform.LookAt(unit.position);
            return true;
        }

        public void Die(Unit from)
        {
            _currentState = UnitState.Dead;
            listener?.OnDead(this, from);
            owner.units.Delete(this);
        }

        private void OnAnimEnd(UnitState state)
        {
            Debug.Log($"{state} : {name}");
            
            switch (state)
            {
                case UnitState.Attack:
                    break;
                case UnitState.Move:
                    break;
                case UnitState.Dead:
                    StartCoroutine(OnDisappearAnim());
                    break;
                case UnitState.Idle:
                    break;
            }
        }

        private void SetFadeMaterialAndAlpha(float alpha)
        {
            for (int i = 0; i < _rendereres.Length; i++)
            {
                _rendereres[i].sharedMaterial = _deadMaterials[i];
                var color = _deadMaterials[i].color;
                color.a = alpha;
                _deadMaterials[i].color = color;
            }
        }
        
        private void SetFadeAlpha(float alpha)
        {
            for (int i = 0; i < _rendereres.Length; i++)
            {
                var color = _deadMaterials[i].color;
                color.a = alpha;
                _deadMaterials[i].color = color;
            }
        }
        
        private IEnumerator<Unit> OnDisappearAnim()
        {
            var duration = 1.0f;
            var startTime = Time.time;

            SetFadeMaterialAndAlpha(1.0f);

            while (startTime + duration >= Time.time)
            {
                var alpha = 1.0f - (Time.time - startTime) / duration;
                SetFadeAlpha(alpha);

                yield return null;
            }

            SetFadeAlpha(0.0f);
            
            listener?.OnClear(this);
        }

        private IEnumerable<Unit> GetNearUnits()
        {
            if (this.region == null)
                return Enumerable.Empty<Unit>();


            var nodes = new List<KG.Map.Region> { this.region };
            nodes.AddRange(this._map.regions[this.region].edges.Select(x => x.data));

            return nodes.SelectMany(x => x.units).Except(new[] { this });
        }

        private IEnumerable<Unit> GetNearUnitsIn(Fix64 range)
        {
            if (this.region == null)
                return Enumerable.Empty<Unit>();

            var regionSize = _map.regionDiagSize;
            var nodes = new HashSet<KG.Map.Region>();
            var q = new Queue<KG.Map.Region>();
            q.Enqueue(this.region);

            while (q.Count > 0)
            {
                var currentRegion = q.Dequeue();

                if ((this.region.centroid.center - currentRegion.centroid.center).sqrMagnitude < (range + regionSize) *
                    (range + regionSize)
                    &&
                    !nodes.Contains(currentRegion))
                {
                    nodes.Add(currentRegion);
                    
                    foreach (var otherRegion in _map.regions[currentRegion].edges)
                    {
                        if (!q.Contains(otherRegion.data) && !nodes.Contains(otherRegion.data))
                        {
                            q.Enqueue(otherRegion.data);
                        }
                    }
                }
            }
            
            return nodes.SelectMany(x => x.units).Except(new[] { this });
        }

        public override string ToString()
        {
            return $"{base.ToString()}({nameof(unitID)}={unitID}, {nameof(unitOriginID)}={unitOriginID})";
        }
    }
}
