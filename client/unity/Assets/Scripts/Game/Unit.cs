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

    public interface IUnit
    {
        void OnDead(Unit unit);
    }
    
    public class Unit : MonoBehaviour, ISelectable, IRenderable, IUnit
    {
        public Shared.Table.Unit table { get; private set; }
        public List<Shared.Table.Skill> skills { get; private set; }
        public Player owner { get; private set; }

        [SerializeField] private int _unitOriginID;
        [SerializeField] private GameObject _highlighted;
        [SerializeField] private Animator _animator;
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

        public float speed 
        {
            get
            {
                var speed = table.Speed;
                if (owner.advanced.TryGetValue(Advanced.UPGRADE_SPEED, out var advanced))
                    speed += advanced;

                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.Speed, out var x))
                    speed += x;

                return speed;
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

        public int maxhp
        {
            get
            {
                var hp = table.Hp;
                var additional = owner.AdditionalStat(unitID);
                if (additional.TryGetValue(StatType.Hp, out var x))
                    hp += x;

                return hp;
            }
        }

        public int hp
        {
            get => _hp;
            set
            {
                _hp = Mathf.Clamp(value, 0, maxhp);
                if (_hp <= 0)
                    _listener?.OnDead(this);
            }
        }

        public List<Skill> activeSkills => skills.Where(x => owner.abilities.HasFlag(x.Condition)).ToList();

        [SerializeField] private uint _teamID;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private uint _unitID;
        [SerializeField] private DateTime _lastAttackTime = DateTime.MinValue;
        [SerializeField] private int _hp;
        [NonSerialized] private IUnit _listener;

        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }

        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [SerializeField] private Renderer[] _rendereres;

        [NonSerialized] private UnitState _currentState;
        [NonSerialized] private UnitState _reservedState;
        
        [NonSerialized] private float _stopMoveDistance;
        [NonSerialized] private Vector3 _moveTargetPosition;
        [NonSerialized] private Unit _targetUnit;

        public Vector3 moveTargetPosition
        {
            get
            {
                if (_targetUnit != null)
                {
                    return _targetUnit.transform.position;
                }
                else
                {
                    return _moveTargetPosition;
                }
            }
        }
        
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
            this.hp = this.maxhp;
            _listener = this;
        }

        private void Update()
        {
            _totalBounds = new Bounds();
            foreach (var renderer in _rendereres)
            {
                _totalBounds.Encapsulate(renderer.bounds);
            }

            switch (_currentState)
            {
                case UnitState.Move:
                    var diff = (moveTargetPosition - transform.position);
                    var magnitude = diff.magnitude;
                    var direction = diff / magnitude;
                    var delta = Time.deltaTime;

                    transform.LookAt(moveTargetPosition);
                
                    if (magnitude < speed * delta || magnitude < _stopMoveDistance + Shared.Const.Character.MoveEpsilon)
                    {
                        SetReservedState();
                    }
                    else 
                    {
                        transform.position += direction * speed * delta;
                    }
                    break;
                case UnitState.Attack:
                    if (ContainsRange(_targetUnit.transform.position))
                    {
                        if (_targetUnit.hp <= 0)
                        {
                            SetReservedState();
                        }
                        else if (Attack(_targetUnit))
                        {
                            _animator.SetTrigger("Attack");
                        }
                    }
                    else
                    {
                        AttackTo(_targetUnit);
                    }
                    break;
            }
        }

        public void Selected(bool select)
        {
            if (_highlighted) _highlighted.SetActive(select);
        }

        public void SetReservedState()
        {
            switch (_currentState = _reservedState)
            {
                case UnitState.Idle:
                    _animator.SetTrigger("Idle");
                    break;
            }
        }
        
        public void MoveTo(Vector3 position)
        {
            _currentState = UnitState.Move;
            _reservedState = UnitState.Idle;
            _targetUnit = null;
            _moveTargetPosition = position;
            _stopMoveDistance = 0.0f;
            
            _animator.SetTrigger("Move");
        }

        public void MoveTo(Unit target, float stopDistance = 0.0f, UnitState reservedState = UnitState.Idle)
        {
            _currentState = UnitState.Move;
            _reservedState = reservedState;
            _targetUnit = target;
            _stopMoveDistance = stopDistance;
            
            _animator.SetTrigger("Move");
        }

        public void AttackTo(Unit target)
        {
            MoveTo(target, attackRange, UnitState.Attack);
        }

        public bool ContainsRange(Vector3 target)
        {
            return (this.transform.position - target).sqrMagnitude < Math.Pow(this.table.AttackRange, 2);
        }

        private int CalculateDamage(Unit unit)
        {
            var result = damage - unit.armor;

            if (table.Type == UnitType.Explosive)
            {
                switch (unit.table.Size)
                {
                    case UnitSize.Small:
                        return result;

                    case UnitSize.Medium:
                        return (int)(result * 0.75);

                    case UnitSize.Large:
                        return (int)(result * 0.5);
                }
            }

            if (table.Type == UnitType.Concussive)
            {
                switch (unit.table.Size)
                {
                    case UnitSize.Large:
                        return result;

                    case UnitSize.Medium:
                        return (int)(result * 0.75);

                    case UnitSize.Small:
                        return (int)(result * 0.5);
                }
            }

            return result;
        }

        public bool Attack(Unit unit)
        {
            var now = DateTime.Now;
            if ((now - _lastAttackTime).TotalMilliseconds < attackSpeed)
                return false;

            unit.hp -= CalculateDamage(unit);
            _lastAttackTime = DateTime.Now;
            return true;
        }

        public void OnDead(Unit unit)
        {
            _currentState = UnitState.Dead;
            _animator.SetTrigger("Dead");
        }
    }
}
