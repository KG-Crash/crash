using FixMath.NET;

namespace Game
{
    public abstract class LogicalObject
    {
        public delegate void RemoveHandler(LogicalObject unit);
        public delegate void PositionChangingHandler(LogicalObject me, FixVector2 from, FixVector2 to);
        public delegate void PositionChangedHandler(LogicalObject me, FixVector2 before, FixVector2 after);
        public delegate void LookAtHandler(LogicalObject me, FixVector3 worldPosition);

        public event RemoveHandler OnRemove;
        public event PositionChangingHandler OnPositionChanging;
        public event PositionChangedHandler OnPositionChanged;
        public event LookAtHandler OnLookAt;

        public int type { get; set; }

        public abstract int damage { get; }

        public abstract Fix64 speed { get; }
        private FixVector3 _position;
        public virtual FixVector3 position
        {
            get => _position;
            set
            {
                OnPositionChanging?.Invoke(this, position, value);
                var before = position;
                _position = value;
                OnPositionChanged?.Invoke(this, before, value);
            }
        }

        protected LogicalObject()
        { }

        public virtual void Destroy()
        {
            OnRemove?.Invoke(this);
        }
    }
}
