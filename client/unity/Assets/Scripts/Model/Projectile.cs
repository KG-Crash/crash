using FixMath.NET;
using Shared;
using Shared.Table;
using UniRx;

namespace Game
{
    public class Projectile : LogicalObject
    {
        public delegate void ReachHandler(Projectile projectile, Unit target);
        public delegate void SpawnedHandler(Projectile projectile);

        public event ReachHandler OnReach;
        public event SpawnedHandler OnSpawned;
        public override event LookAtHandler OnLookAt;

        public Projectile(uint uniqueID, Unit from, Unit to)
        {
            this.uniqueID = uniqueID;
            type = from.projectileType;
            owner = from;
            target = to;

            info = Table.From<TableProjectile>()[type];
            currentState = ProjectileState.Shoot;
            
            OnSpawned?.Invoke(this);
            GameController.updateFrameStream.Subscribe(Action);
            // TODO :: 풀링을 핟면 Disable 해야함.
            // Disable();
            Shoot();
        }

        public int type { get; private set; }
        public uint uniqueID { get; private set; }

        public FixVector3 position { get; set; }

        public Shared.Table.Projectile info { get; private set; }

        private ProjectileState _currentState;
        private FixVector3? _moveTarget;
        private int _damage;

        private Player _owner;

        public override int damage => _damage;
        public override Fix64 speed => info.Speed;
        public bool targeting => info.Targeting;
        
        public Unit owner { get; set; }

        public Unit target { get; set; }

        public ProjectileState currentState
        {
            get => _currentState;
            set => _currentState = value;
        }

        public FixVector3? moveTarget
        {
            get
            {
                if (targeting)
                    return target?.position ?? _moveTarget;
                else
                    return _moveTarget;
            }
            set => _moveTarget = value;
        }

        private void Action(Frame f)
        {
            switch (currentState)
            {
                case ProjectileState.Disable:
                    break;
                case ProjectileState.Shoot:
                    break;
                case ProjectileState.Move:
                    DeltaMove(f.deltaTime);
                    break;
                case ProjectileState.Hit:
                    break;
            }
        }
        
        public void Disable()
        {
            currentState = ProjectileState.Disable;
            target = null;
            owner = null;
            moveTarget = null;
        }
        
        public void Shoot()
        {
            moveTarget = target.position;
            currentState = ProjectileState.Move;            
        }
        
        public void Hit(Unit target)
        {
            currentState = ProjectileState.Hit;
            
            OnReach?.Invoke(this, target);
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
                OnLookAt?.Invoke(this, moveTarget.Value); // 이거 뭐냐

                var arrived = magnitude < (speed * delta);
                if (arrived)
                    Hit(target);
                else
                    position += (direction * speed * delta);
            }
            else
                Hit(target);
        }
    }
}