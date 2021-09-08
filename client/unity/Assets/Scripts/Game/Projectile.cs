using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;
using Shared.Table;
using System;
using FixMath.NET;

namespace Game
{
    public class Projectile : MonoBehaviour
    {
        public interface Listener
        {
            void OnProjectileReach(Unit owner, Projectile projectile);
        }

        [SerializeField] private int _projectileOriginID; 
        [SerializeField] private uint _projectileID;

        //[SerializeField] public Animator _animator;
        //[SerializeField] private Rigidbody _rigidbody;
        //[SerializeField] private Bounds _bounds = new Bounds();
        //[SerializeField] private Renderer[] _renderers;

        [SerializeField] private ProjectileState _currentState;
        [NonSerialized] private FixVector3? _moveTarget;

        [NonSerialized] private Unit? _target;
        [SerializeField] private Unit? _owner;

        [NonSerialized] private Listener _listener;

        public FixVector3 position { get; set; }


        public Shared.Table.Projectile table { get; private set; }

        public int projectileOriginID
        {
            get => _projectileOriginID;
        }
        public uint projectileID
        {
            get => _projectileID;
            set => _projectileID = value;
        }
        public Fix64 speed
        {
            get
            {
                var speed = table.Speed;
                return (Fix64)speed;
            }
        }

        public bool type
        {
            get
            {
                var type = table.Targeting;
                return type;
            }
        }

        public Unit owner
        {
            get => _owner;
            set => _owner = value;
        }

        public Unit target
        {
            get => _target;
            set => _target = value;
        }

        public FixVector3? moveTarget
        {
            get
            {
                if (this.type.Equals(true))
                    return _target?.position ?? _moveTarget;
                else
                    return _moveTarget;
            }
            set => _moveTarget = value;
        }
        public Listener listener
        {
            get => _listener;
            set => _listener = value;
        }


        private void Awake()
        {
            this.table = Table.From<TableProjectile>()[this._projectileOriginID];
            _currentState = ProjectileState.Shoot;
        }

        // Update is called once per frame
        private void Update()
        {
            Action(); 

            this.transform.position = this.position;
        }

        private void Action()
        {
            switch (_currentState)
            {
                case ProjectileState.Disable:
                    break;
                case ProjectileState.Shoot:
                    break;

                case ProjectileState.Move:
                    DeltaMove((Fix64)Time.deltaTime);
                    break;

                case ProjectileState.Hit:
                    listener?.OnProjectileReach(owner, this);
                    break;
            }
        }
        public void Disable()
        {
            _currentState = ProjectileState.Disable;
            target = null;
            owner = null;
            moveTarget = null;

        }
        public void Shoot()
        {
            moveTarget = target.position;
            _currentState = ProjectileState.Move;            
        }
        public void Hit()
        {
            _currentState = ProjectileState.Hit;
        }

        private void DeltaMove(Fix64 delta)
        {
            if (moveTarget == null)
                return;

            var diff = (moveTarget.Value - position);
            var magnitude = diff.magnitude;

            if (magnitude != Fix64.Zero)
            {
                var direction = diff / magnitude;
                transform.LookAt(moveTarget.Value);

                var arrived = magnitude < (speed * delta);
                if (arrived)
                    Hit();
                else
                    position += (direction * speed * delta);
            }
            else
                Hit();
        }        
    }
}