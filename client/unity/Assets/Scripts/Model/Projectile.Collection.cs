using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class ProjectileCollection : IReadOnlyDictionary<uint, Projectile>
    {
        #region override methods
        private Dictionary<uint, Projectile> _projectiles = new Dictionary<uint, Projectile>();

        public Projectile this[uint key]
        {
            get
            {
                if (this._projectiles.TryGetValue(key, out var projectile) == false)
                    return null;

                return projectile;
            }
        }

        public IEnumerable<uint> Keys => this._projectiles.Keys;

        public IEnumerable<Projectile> Values => this._projectiles.Values;

        public int Count => this._projectiles.Count;

        public bool ContainsKey(uint key) => this._projectiles.ContainsKey(key);

        public IEnumerator<KeyValuePair<uint, Projectile>> GetEnumerator() => this._projectiles.GetEnumerator();

        public bool TryGetValue(uint key, out Projectile value) => this._projectiles.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => this._projectiles.GetEnumerator();
        #endregion

        #region private fields
        private Unit _unit;
        private Projectile.Listener _listener;
        #endregion

        public ProjectileCollection(Unit unit, Projectile.Listener listener)
        {
            _unit = unit;
            _listener = listener;
        }

        public Projectile Add(uint id, Unit target)
        {
            if (this._projectiles.ContainsKey(id))
                throw new Exception("asd");

            var projectile = new Projectile(id, _unit, target, _listener);
            this._projectiles.Add(id, projectile);
            return projectile;
        }

        public void Remove(uint id)
        {
            if (this._projectiles.ContainsKey(id) == false)
                return;

            this._projectiles.Remove(id);
        }
    }
}