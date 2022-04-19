using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class LogicalObject
    {
        public interface Listener
        {
            void OnRemove(LogicalObject unit);
            void OnPositionChanging(LogicalObject me, FixVector2 from, FixVector2 to);
            void OnPositionChanged(LogicalObject me, FixVector2 before, FixVector2 after);
            void OnLookAt(LogicalObject me, FixVector3 direction);
        }

        private Listener _listener;
        
        public int type { get; set; }

        public abstract int damage { get; }

        public abstract Fix64 speed { get; }
        private FixVector3 _position;
        public virtual FixVector3 position
        {
            get => _position;
            set
            {
                _listener?.OnPositionChanging(this, position, value);
                var before = position;
                _position = value;
                _listener?.OnPositionChanged(this, before, value);
            }
        }

        protected LogicalObject(Listener listener)
        {
            _listener = listener;
        }

        public virtual void Destroy()
        {
            _listener?.OnRemove(this);
        }
    }
}
