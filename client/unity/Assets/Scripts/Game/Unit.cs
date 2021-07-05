using FixMath.NET;
using Shared;
using Shared.Table;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public Shared.Table.Unit table { get; private set; }
        public List<Shared.Table.Skill> skills { get; private set; }
        

        [SerializeField] private int _unitOriginID;
        [SerializeField] private GameObject _highlighted;
        [SerializeField] public Animator animator;
        [SerializeField] private Rigidbody _rigidbody;

        public uint teamID
        {
            get => _teamID;
            set => _teamID = value;
        }

        public bool selectable
        {
            get => _selectable;
            set => _selectable = value;
        }

        public int unitOriginID
        {
            get => _unitOriginID;
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

        public Fix64 hp
        {
            get => _hp;
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



        public List<Skill> activeSkills => skills.Where(x => owner.abilities.HasFlag(x.Condition)).ToList();

        [SerializeField] private uint _teamID;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private uint _unitID;
        [SerializeField] private DateTime _lastAttackTime = DateTime.MinValue;
        [SerializeField] private Fix64 _hp;
        [SerializeField] private Player _owner;
        [NonSerialized] private Listener _listener;

        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }

        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [SerializeField] private Renderer[] _rendereres;

        [NonSerialized] private UnitState _currentState;
        [NonSerialized] private bool _watchEnemy; // 이동중에 적군을 만나면 공격을 할지 말지
        
        [NonSerialized] private float _stopMoveDistance;
        [NonSerialized] private FixVector3? _moveTargetPosition;
        [NonSerialized] private Unit _target;

        public FixVector3 position { get; set; }

        public FixVector3? moveTargetPosition => _target?.position ?? _moveTargetPosition;

        public bool IsDead => _hp == Fix64.Zero;

        [ContextMenu("Gather renderers")]
        private void OnRefreshRenderers()
        {
            _rendereres = GetComponentsInChildren<Renderer>();
        }

        public Unit()
        {
            // TODO : 제거해야함 테스트코드임
            this.owner = new Player();
            this.owner.SetAbilityFlag(Ability.UPGRADE_4 | Ability.UPGRADE_10);
            this.table = Table.From<TableUnit>()[this._unitOriginID];
            this.skills = Table.From<TableSkill>().Values.Where(x => x.Unit == this.unitOriginID).ToList();
            this._hp = this.maxhp;
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
                    _target = SearchEnemy();
                    if (_target != null)
                        _currentState = UnitState.Attack;
                    break;

                case UnitState.Move:
                    if (_watchEnemy == false)
                    {
                        // 강제이동
                        DeltaMove((Fix64)Time.deltaTime);
                    }
                    else if (_target == null)
                    {
                        // 어택땅
                        _target = SearchEnemy();
                        if (_target != null)
                            _currentState = UnitState.Attack;
                        else
                            DeltaMove((Fix64)Time.deltaTime);
                    }
                    else
                    {
                        // 특정유닛 공격
                        if (ContainsRange(_target.position))
                            _currentState = UnitState.Attack;
                    }
                    break;

                case UnitState.Attack:
                    if (ContainsRange(_target.position))
                    {
                        Attack(_target);
                        if (_target.IsDead)
                        {
                            _target = null;

                            if (_moveTargetPosition == null) // 상대도 죽였는데 이동할 곳도 없음
                            {
                                _currentState = UnitState.Idle;
                            }
                            else // 갈 곳이 있음 => 어택땅 상태임
                            {
                                _currentState = UnitState.Move;
                            }
                        }
                    }
                    else
                    {
                        AttackTo(_target);
                    }
                    break;
            }
        }

        private void DeltaMove(Fix64 delta)
        {
            if (moveTargetPosition == null)
                return;

            var diff = (moveTargetPosition.Value - position);
            var magnitude = diff.magnitude;
            if (magnitude != Fix64.Zero)
            {
                var direction = diff / magnitude;

                // TODO : 이거는 나중에 동기화 때 처리해야 할 문제 (Time.deltaTime을 사용하지 않아야 함)
                transform.LookAt(moveTargetPosition.Value);

                var arrived = magnitude < (speed * delta) || magnitude < (Fix64)_stopMoveDistance + (Fix64)Shared.Const.Character.MoveEpsilon;
                if (arrived)
                {
                    Stop();
                }
                else
                {
                    position += (direction * speed * delta);
                }
            }
            else
            {
                Stop();
            }
        }

        private Unit SearchEnemy()
        {
            // TODO : 내 시야 범위 안에 있는 유닛 찾아내기
            // 섹터 개념 도입 후 실제 적용
            return null;
        }

        public void Stop()
        {
            _currentState = UnitState.Idle;
            _moveTargetPosition = null;
            listener?.OnEndMove(this);
        }

        public void Selected(bool select)
        {
            if (_highlighted) _highlighted.SetActive(select);
        }

        public void MoveTo(FixVector3 position)
        {
            _currentState = UnitState.Move;
            _target = null;
            _moveTargetPosition = position;
            _stopMoveDistance = 0.0f;

            listener?.OnStartMove(this);
        }

        public void MoveTo(Unit target, float stopDistance = 0.0f)
        {
            _currentState = UnitState.Move;
            _target = target;
            _stopMoveDistance = stopDistance;
            _moveTargetPosition = null;

            listener?.OnStartMove(this);
        }

        public void AttackTo(Unit target)
        {
            _watchEnemy = true;
            _target = target;
            if (ContainsRange(target.position))
            {
                _currentState = UnitState.Attack;
            }
            else
            {
                MoveTo(target, attackRange);
            }
        }

        public void AttackTo(FixVector3 position)
        {
            _watchEnemy = true;
            MoveTo(position);
        }

        public bool ContainsRange(FixVector3 target)
        {
            return (position - target).sqrMagnitude < (Fix64)Math.Pow(this.table.AttackRange, 2);
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

#if DEBUG
            return Fix64.MaxValue;
#else
            return result;
#endif
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

            if (_hp == Fix64.Zero)
                Die(from);
        }

        public bool Attack(Unit unit)
        {
            var now = DateTime.Now;
            if ((now - _lastAttackTime).TotalMilliseconds < attackSpeed)
                return false;

            var damage = CalculateDamage(unit);
            unit.AddHP(-damage, this);
            listener?.OnAttack(this, unit, damage);
            _lastAttackTime = DateTime.Now;

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
                    break;
                case UnitState.Idle:
                    break;
            }
        }
    }
}
