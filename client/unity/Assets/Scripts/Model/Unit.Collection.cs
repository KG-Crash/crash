using FixMath.NET;
using KG;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class UnitCollection : IEnumerable<Unit>
    {
        public static uint SEUQENCE = 0;

        private readonly Player _owner;
        private readonly Dictionary<uint, Unit> _units = new Dictionary<uint, Unit>();
        private Unit.Listener _listener;

        public UnitCollection(Player owner, Unit.Listener listener)
        {
            _owner = owner;
            _listener = listener;
        }

        public Unit this[uint i]
        {
            get
            {
                if (_units.TryGetValue(i, out var unit))
                    return unit;
                else
                    return null;
            }
            set
            {
                if (_units.ContainsKey(i))
                    throw new System.Exception("duplicate key");

                _units.Add(i, value);
            }
        }

        public void Add(int type, Map map, FixVector2 position)
        {
            var unit = new Unit(SEUQENCE++, type, map, _owner, position, _listener);

            if (unit.owner != null)
            {
                // unit.owner.units.Delete 하면 안됨
                // OnOwnerChanged가 2번 호출됨
                unit.owner.units._units.Remove(unit.uniqueID);
            }

            unit.owner = this._owner;
            _units.Add(unit.uniqueID, unit);
        }

        public void Delete(uint unitId)
        {
            _units[unitId].owner = null;
            _units.Remove(unitId);
        }

        public void Delete(Unit unit)
        {
            unit.owner = null;
            _units.Remove(unit.uniqueID);
        }

        public Dictionary<uint, Unit>.ValueCollection Values => _units.Values;


        public IEnumerator<Unit> GetEnumerator()
        {
            foreach (var pair in _units)
            {
                yield return pair.Value;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
