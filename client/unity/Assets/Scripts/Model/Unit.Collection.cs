using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class UnitCollection : IEnumerable<Unit>
    {
        private readonly Player _owner;
        private readonly Dictionary<uint, Unit> _units = new Dictionary<uint, Unit>();

        public UnitCollection(Player owner)
        {
            _owner = owner;
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

        public void Add(Unit unit)
        {
            if (unit.owner != null)
            {
                // unit.owner.units.Delete 하면 안됨
                // OnOwnerChanged가 2번 호출됨
                unit.owner.units._units.Remove(unit.uniqueID);
            }

            unit.owner = this._owner;
            _units.Add(unit.uniqueID, unit);
        }

        public void AddRange(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                Add(unit);
            }
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
