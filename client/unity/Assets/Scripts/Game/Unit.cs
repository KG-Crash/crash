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
        public Shared.Table.Unit Table { get; private set; }
        public Player Owner { get; private set; }

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

        public int Damage => Table.Damage + (Owner?.Advanced[Shared.Advanced.UPGRADE_WEAPON] ?? 0);

        public int Armor => Table.Armor + (Owner?.Advanced[Shared.Advanced.UPGRADE_ARMOR] ?? 0);

        [SerializeField] private int _team;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private int _unitID;
        
        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }

        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [SerializeField] private Renderer[] _rendereres;

        [ContextMenu("Gather renderers")]
        private void OnRefreshRenderers()
        {
            _rendereres = GetComponentsInChildren<Renderer>();
        }

        public Unit()
        {
            this.Table = Shared.Table.Table.From<TableUnit>()[this._unitOriginID];
        }

        private void Update()
        {
            _totalBounds = new Bounds();
            foreach (var renderer in _rendereres)
            {
                _totalBounds.Encapsulate(renderer.bounds);
            }
        }

        public void Selected(bool select)
        {
            if (_highlighted) _highlighted.SetActive(select);
        }

        public void MoveTo(Vector3 position)
        {
        }

        public bool ContainsRange(Vector3 target)
        {
            return (this.transform.position - target).sqrMagnitude < Math.Pow(this.Table.AttackRange, 2);
        }
    }
}