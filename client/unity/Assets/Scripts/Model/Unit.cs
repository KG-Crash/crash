using FixMath.NET;
using Shared;
using Shared.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Game
{
    public partial class Unit : LogicalObject
    {
        public delegate void AttackHandler(Unit me, Unit you, Fix64 damage);
        public delegate void HealHandler(Unit me, Unit you, Fix64 heal);
        public delegate void DamagedHandler(Unit me, Unit you, Fix64 damage);
        public delegate void DeadHandler(Unit unit, Unit from);
        public delegate void OwnerChangedHandler(Player before, Player after, Unit unit);
        public delegate void StartMoveHandler(Unit unit);
        public delegate void EndMoveHandler(Unit unit);
        public delegate void FireProjectileHandler(Unit fireUnit, Unit you, int projectileOriginID);
        public delegate void IdleHandler(Unit unit);
        public delegate void UpdateHandler(Unit me, Frame f);
        public delegate void HPChangingHandler(Unit me, int from, int to);
        public delegate void HPChangedHandler(Unit me, int before, int after);
        public delegate void SpawnedHandler(Unit me);

        public event AttackHandler OnAttack;
        public event HealHandler OnHeal;
        public event DamagedHandler OnDamaged;
        public event DeadHandler OnDead;
        public event OwnerChangedHandler OnOwnerChanged;
        public event StartMoveHandler OnStartMove;
        public event EndMoveHandler OnEndMove;
        public event FireProjectileHandler OnFireProjectile;
        public event IdleHandler OnIdle;
        public event UpdateHandler OnUpdate;
        public event HPChangingHandler OnHPChanging;
        public event HPChangedHandler OnHPChanged;
        public event SpawnedHandler OnSpawned;


        public Shared.Table.Unit info => Table.From<TableUnit>()[type];
        public List<Shared.Table.Skill> skills => Table.From<TableSkill>().Values.Where(x => x.Unit == type).ToList();

        public Team team => owner.team;

        public bool selectable
        {
            get => _selectable;
            set => _selectable = value;
        }

        public uint uniqueID
        {
            get => _uniqueID;
            set => _uniqueID = value;
        }

        public override int damage
        {
            get
            {
                var damage = info.Damage;
                if (owner.advanced.TryGetValue(Advanced.UPGRADE_WEAPON, out var advanced))
                    damage += advanced;

                var additional = owner.AdditionalStat(type);
                if (additional.TryGetValue(StatType.Damage, out var x))
                    damage += x;

                return damage;
            }
        }

        public int armor
        {
            get
            {
                var armor = info.Armor;
                if (owner.advanced.TryGetValue(Advanced.UPGRADE_ARMOR, out var advanced))
                    armor += advanced;

                var additional = owner.AdditionalStat(type);
                if (additional.TryGetValue(StatType.Armor, out var x))
                    armor += x;

                return armor;
            }
        }

        public override Fix64 speed
        {
            get
            {
                var speed = info.Speed;
                if (owner.advanced.TryGetValue(Advanced.UPGRADE_SPEED, out var advanced))
                    speed += advanced;

                var additional = owner.AdditionalStat(type);
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
                var attackSpeed = info.AttackSpeed;

                var additional = owner.AdditionalStat(type);
                if (additional.TryGetValue(StatType.AttackSpeed, out var x))
                    attackSpeed += x;

                return attackSpeed;
            }
        }

        public int attackRange
        {
            get
            {
                var attackRange = info.AttackRange;

                var additional = owner.AdditionalStat(type);
                if (additional.TryGetValue(StatType.AttackRange, out var x))
                    attackRange += x;

                return attackRange;
            }
        }

        public AttackType attackType
        {
            get => info.AttackType;
        }

        public int projectileType
        {
            get => info.ProjectileID;
        }

        public Fix64 maxhp
        {
            get
            {
                var hp = info.Hp;
                var additional = owner.AdditionalStat(type);
                if (additional.TryGetValue(StatType.Hp, out var x))
                    hp += x;

                return new Fix64(hp);
            }
        }

        public Fix64 visibleRange
        {
            get => info.VisibleRange;
        }

        public Fix64 hp
        {
            get => _hp;
            set 
            {
                OnHPChanging?.Invoke(this, this._hp, value);
                var before = this._hp;
                _hp = value;
                OnHPChanged?.Invoke(this, before, value);
            }
        }

        public int killScore
        {
            get => info.KillScore;
        }

        public Player owner
        {
            get => _owner;
            set
            {
                var before = _owner;
                _owner = value;
                OnOwnerChanged?.Invoke(before, _owner, this);
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

        public ProjectileCollection projectiles;

        public List<Skill> activeSkills => skills.Where(x => owner.upgrade.abilities.HasFlag(x.Condition)).ToList();

        [SerializeField] private uint _teamID;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private uint _uniqueID;
        [SerializeField] private Fix64 _hp;
        [SerializeField] private Player _owner;
        [NonSerialized] private KG.Map _map;
        
        [NonSerialized] public UnitState _currentState;
        [NonSerialized] public Unit _target;
        [NonSerialized] public HashSet<Unit> _attackers = new HashSet<Unit>();
        [NonSerialized] private int _lastAttackFrame;

        public void AddAttacker(Unit unit)
        {
            _attackers.Add(unit);
        }

        public override FixVector3 position 
        {
            get => base.position;
            set
            {
                region = map[value]?.region;

                // 길찾기 경로에서 현재 경로 제거
                _regionPath.Remove(region);

                base.position = value;
            }
        }

        public FixVector2 size => new FixVector2(new Fix64(info.Width) / new Fix64(10000), new Fix64(info.Height) / new Fix64(10000));

        public FixRect collisionBox => GetCollisionBox(position);
        public IEnumerable<KG.Map.Cell> collisionCells => GetCollisionCells(position);

        public bool IsDead => _currentState == UnitState.Dead || _hp == Fix64.Zero;

        public static bool IsNullOrDead(Unit unit)
        {
            return unit == null || unit.IsDead;
        }

        public static FixRect GetCollisionBox(int type, FixVector2 position)
        {
            var info = Table.From<TableUnit>()[type];
            var size = new FixVector2(new Fix64(info.Width) / new Fix64(10000), new Fix64(info.Height) / new Fix64(10000));
            var half = size / new Fix64(2);
            return new FixRect(position.x - half.x, position.y - half.y, size.x, size.y);
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

        public IEnumerable<KG.Map.Cell> GetCollisionCells(FixVector2 position)
        {
            return GetCollisionCells(map, type, position);
        }

        public static IEnumerable<KG.Map.Cell> GetCollisionCells(KG.Map map, int type, FixVector2 position)
        {
            var collisionBox = GetCollisionBox(type, position);
            var min = new FixVector2(collisionBox.minX, collisionBox.minY);
            var max = new FixVector2(collisionBox.maxX, collisionBox.maxY);

            var cells = new List<KG.Map.Cell>();
            var begin = map[min];
            if (begin == null)
                yield break;

            var end = map[max];
            if (end == null)
                yield break;

            for (int row = begin.row; row <= end.row; row++)
            {
                for (int col = begin.col; col <= end.col; col++)
                {
                    yield return map[row, col];
                }
            }

            yield break;
        }

        public bool IsWalkable(KG.Map.Cell cell)
        {
            return GetCollisionCells(cell.position).All(x => x.walkable);
        }

        public Unit(uint uniqueID, int type, KG.Map map, Player owner, FixVector3 position)
        {
            this.type = type;
            this.uniqueID = uniqueID;
            _map = map;
            _owner = owner;
            hp = maxhp;
            projectiles = new ProjectileCollection(this);

            _lastAttackFrame = -attackSpeed / new Fix64(1000) / GameController.TimeDelta;

            GameController.updateFrameStream.Subscribe(Action);

            OnSpawned?.Invoke(this);
            this.position = position;
        }

        private void TransitionTo(UnitState state)
        {
            if (_currentState == state)
            {
                return;   
            }

            var prevState = state;
            _currentState = state;
            
            if (prevState == UnitState.Move)
                OnEndMove?.Invoke(this);

            switch (_currentState)
            {
                case UnitState.Idle:
                    OnIdle?.Invoke(this);
                    break;
                case UnitState.Move:
                    OnStartMove?.Invoke(this);
                    break;
                case UnitState.Attack:
                    Attack(_target);
                    break;
            }
        }
        
        private bool TryGetNearAttackerInAttackRange(out Unit unit)
        {
            unit = _attackers
                .Where(x => !IsNullOrDead(x) && ContainsAttackRange(x.position))
                .OrderBy(x => (x.position - position).sqrMagnitude)
                .FirstOrDefault();
            return unit != null;
        }
        
        private Unit GetNearAttacker()
        {
            return _attackers
                .Where(x => !IsNullOrDead(x))
                .OrderBy(x => (x.position - position).sqrMagnitude)
                .FirstOrDefault();
        }
        
        private void Action(Frame f)
        {
            switch (_currentState)
            {
                case UnitState.Idle:
                    if (IsNullOrDead(_target))
                    {
                        _target = GetNearAttacker();
                        
                        if (_target == null)
                            _target = SearchEnemyIn(visibleRange);
                    }

                    if (!IsNullOrDead(_target))
                    {
                        if (ContainsAttackRange(_target.position))
                        {
                            TransitionTo(UnitState.Attack);
                        }
                        else
                        {
                            if (remainImmediatePath)
                                TransitionTo(UnitState.Move);
                            else if (remainPath && TryUpdateMovePath())
                            {
                                TransitionTo(UnitState.Move);
                            }
                        }
                    }
                    else 
                    {
                        if (remainImmediatePath)
                        {
                            TransitionTo(UnitState.Move);
                        }
                        else if (remainPath)
                        {
                            if (TryUpdateMovePath())
                            {
                                TransitionTo(UnitState.Move);   
                            }
                        }
                    }
                    break;

                case UnitState.Move:
                    if (IsNullOrDead(_target))
                    {
                        _target = GetNearAttacker();
                        
                        if (IsNullOrDead(_target))
                        {
                            _target = SearchEnemyIn(attackRange);
                        }
                    }
                    else if (!ContainsAttackRange(_target.position))
                    {
                        if (TryGetNearAttackerInAttackRange(out var inRangeUnit))
                        {
                            _target = inRangeUnit;   
                        }
                    }
                    
                    // 공격 가능?
                    if (!IsNullOrDead(_target) && ContainsAttackRange(_target.position))
                    {
                        TransitionTo(UnitState.Attack);
                    }
                    else if (!remainImmediatePath)
                    {
                        TransitionTo(UnitState.Idle);
                    }
                    else
                    {   
                        DeltaMove(f);
                    }
                    break;

                case UnitState.Attack:
                    if (IsNullOrDead(_target))
                    {
                        if (TryGetNearAttackerInAttackRange(out var inRangeUnit))
                            _target = inRangeUnit;

                        if (IsNullOrDead(_target))
                            _target = SearchEnemyIn(attackRange);
                    }
                    else if (!ContainsAttackRange(_target.position))
                    {
                        if (TryGetNearAttackerInAttackRange(out var inRangeUnit))
                        {
                            _target = inRangeUnit;   
                        }
                    }

                    if (IsNullOrDead(_target) || !ContainsAttackRange(_target.position))
                    {
                        var canMoveNow = TryUpdateMovePath();
                        var targetState = canMoveNow ? UnitState.Move : UnitState.Idle;
                        TransitionTo(targetState);
                    }
                    else
                    {
                        Attack(_target);   
                    }
                    break;
            }

            OnUpdate?.Invoke(this, f);
        }

        private Unit SearchEnemyIn(Fix64 searchRadius)
        {
            return GetNearUnitsIn(searchRadius)
                .Where(x => !IsNullOrDead(x) && x.team != team)
                .OrderBy(x => (x.position - position).sqrMagnitude)
                .FirstOrDefault(x => ContainsRange(x.position, searchRadius));
        }

        public void Stop()
        {
            ClearPathAndDestination();
            TransitionTo(UnitState.Idle);

            UnityEngine.Debug.Log("stop moving");
        }

        public void MoveTo(FixVector3 position)
        {
            _target = null;

            var targetState = 
                ResetMovePathAndDestination(position) ? UnitState.Move : UnitState.Idle;
            TransitionTo(targetState);
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

            if (info.Type == UnitType.Explosive)
            {
                switch (unit.info.Size)
                {
                    case UnitSize.Small:
                        return result;

                    case UnitSize.Medium:
                        return result * (Fix64)0.75;

                    case UnitSize.Large:
                        return result * (Fix64)0.5;
                }
            }

            if (info.Type == UnitType.Concussive)
            {
                switch (unit.info.Size)
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
            hp = Fix64.Clamp(Fix64.Zero, maxhp, hp + value);

            if (value < Fix64.Zero)
                OnDamaged?.Invoke(this, from, -value);
            else
                OnHeal?.Invoke(this, from, value);

            if (_hp <= Fix64.Zero)
                Die(from);
        }

        public bool Attack(Unit unit)
        {
            var currentFrame = GameController.InputTotalFrame;
            if ((currentFrame - _lastAttackFrame) * GameController.TimeDelta < attackSpeed / new Fix64(1000))
                return false;

            var damage = CalculateDamage(unit);

            if (attackType == AttackType.Immediately)
            {
                unit.AddHP(-damage, this);
                OnAttack?.Invoke(this, unit, damage);
            }
            else
            {
                OnFireProjectile?.Invoke(this, unit, projectileType);
            }

            
            _lastAttackFrame = currentFrame;

            _attackers.Clear();
            OnLookAt?.Invoke(this, unit.position);
            return true;
        }

        public void Die(Unit from)
        {
            _currentState = UnitState.Dead;
            OnDead?.Invoke(this, from);
            owner.units.Delete(this);
            region.units.Remove(this);
        }

        private IEnumerable<Unit> GetNearUnits()
        {
            if (region == null)
                return Enumerable.Empty<Unit>();


            var nodes = new List<KG.Map.Region> { region };
            nodes.AddRange(_map.regions[region].edges.Select(x => x.data));

            return nodes.SelectMany(x => x.units).Except(new[] { this });
        }

        private IEnumerable<Unit> GetNearUnitsIn(Fix64 range)
        {
            if (region == null)
                return Enumerable.Empty<Unit>();

            var regionSize = _map.regionDiagSize;
            var nodes = new HashSet<KG.Map.Region>();
            var q = new Queue<KG.Map.Region>();
            q.Enqueue(region);

            while (q.Count > 0)
            {
                var currentRegion = q.Dequeue();

                if ((region.centroid.center - currentRegion.centroid.center).sqrMagnitude < (range + regionSize) *
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
            return $"{base.ToString()}({nameof(uniqueID)}={uniqueID}, {nameof(type)}={type})";
        }

        public override void Destroy()
        {
            _owner.units.Delete(this);
            base.Destroy();
        }
    }
}
