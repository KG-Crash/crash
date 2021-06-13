using Shared;
using Shared.Table;
using System;
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
        public Shared.Table.Unit table { get; private set; }
        public Player owner { get; private set; }

        [SerializeField] private int _unitOriginID;
        [SerializeField] private GameObject _highlighted;
        [SerializeField] private Animator _animator;

        public int team
        {
            get => _team;
            set => _team = value;
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
        
        public int unitID
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
        [SerializeField] private int _team;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private int _unitID;
        
        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }

        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [SerializeField] private Renderer[] _rendereres;

        [NonSerialized] private bool moveTo;
        [NonSerialized] private Vector3 _moveTarget;
        
        [ContextMenu("Gather renderers")]
        private void OnRefreshRenderers()
        {
            _rendereres = GetComponentsInChildren<Renderer>();
        }

        public Unit()
        {
            // TODO : �����ؾ��� �׽�Ʈ�ڵ���
            this.owner = new Player();
            this.owner.abilities |= (Ability.UPGRADE_4 | Ability.UPGRADE_10);
            this.table = Shared.Table.Table.From<TableUnit>()[this._unitOriginID];
            UnityEngine.Debug.Log(damage);
        }

        private void Update()
        {
            _totalBounds = new Bounds();
            foreach (var renderer in _rendereres)
            {
                _totalBounds.Encapsulate(renderer.bounds);
            }

            if (moveTo)
            {
                var diff = (_moveTarget - transform.position);
                var magnitude = diff.magnitude;
                var direction = diff / magnitude;
                var delta = Time.deltaTime;

                if (speed * delta < magnitude + Shared.Const.Character.MoveEpsilon)
                {
                    transform.position += direction * speed * delta;
                }
                else
                {
                    transform.position = _moveTarget;
                    moveTo = false;
                }
            }
        }

        public void Selected(bool select)
        {
            if (_highlighted) _highlighted.SetActive(select);
        }

        public void MoveTo(Vector3 position)
        {
            moveTo = true;
            _moveTarget = position;
        }

        public bool ContainsRange(Vector3 target)
        {
            return (this.transform.position - target).sqrMagnitude < Math.Pow(this.table.AttackRange, 2);
        }
    }
}