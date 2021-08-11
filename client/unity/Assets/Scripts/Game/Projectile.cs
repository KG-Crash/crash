using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;
using System;
using FixMath.NET;

namespace Game
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private int _projectileOriginID;
        [SerializeField] public Animator _animator;
        [SerializeField] private Rigidbody _rigidbody;

        [SerializeField] private Bounds _bounds = new Bounds();
        [SerializeField] private Renderer[] _renderers;

        [SerializeField] private uint _ownerUnitID;
        [NonSerialized] private ProjectileState _currentState;

        [NonSerialized] private FixVector3? _origin;
        [NonSerialized] private FixVector3? _target;


        public Shared.Table.Projectile _table { get; private set; }

        public int projectileOriginID
        {
            get => _projectileOriginID;
        }
        public Fix64 speed
        {
            get
            {
                var speed = _table.Speed;
                return (Fix64)speed;
            }
        }

        public bool targeting
        {
            get
            {
                var targeting = _table.Targeting;
                return targeting;
            }
        }

        public uint ownerUnitID
        {
            get
            {
                return _ownerUnitID;
            }
            set
            {
                _ownerUnitID = value;
            }
        }


        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}
