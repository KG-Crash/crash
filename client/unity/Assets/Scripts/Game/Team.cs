using Shared;
using Shared.Table;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Team
    {
        public Dictionary<uint, List<Player>> players { get; private set; } = new Dictionary<uint, List<Player>>();
    }

    public class Player
    {
        #region Upgrade
        /// <summary>
        /// 업그레이드 상태
        /// </summary>
        public Ability abilities { get; private set; } = Ability.NONE;
        
        /// <summary>
        /// 누적되는 업그레이드 상태
        /// </summary>
        public Dictionary<Advanced, int> advanced { get; private set; } = new Dictionary<Advanced, int>();
        #endregion

        /// <summary>
        /// 보유 유닛
        /// </summary>
        //public Dictionary<uint, Unit> units { get; private set; } = new Dictionary<uint, Unit>();
        public UnitCollection units;

        public Player()
        {
            units = new UnitCollection(this);
        }

        public Dictionary<StatType, int> AdditionalStat(uint unitID)
        {
            return Shared.Table.Table
                    .From<TableUnitUpgrade>()
                    .Where(x => abilities.HasFlag(x.Key))
                    .SelectMany(x => x.Value)
                    .Where(x => x.Unit == unitID)
                    .SelectMany(x => x.Additional)
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Sum(x => x.Value));
        }

        public Ability SetAbilityFlag(Ability ability)
        {
            abilities |= ability;
            return abilities;
        }
    }

    public class UnitCollection : IEnumerable<Unit>
    {
        private Player _owner;
        private Dictionary<uint, Unit> _units = new Dictionary<uint, Unit>();
        public UnitCollection(Player owner)
        {
            _owner = owner;
        }

        public Unit this[uint i]
        {
            get { return _units[i]; }
            set { _units[i] = value; }
        }

        public void Add(Unit unit)
        {
            unit.owner = this._owner;
            _units.Add(unit.unitID, unit);
        }
        public void AddRange(IEnumerable<Unit> units)
        {
            foreach (Unit unit in units)
            {
                unit.owner = this._owner;
            }
            _units =
                _units.Union(units.ToDictionary(u => u.unitID, u => u)).
                ToDictionary(u => u.Key, u => u.Value);
        }
        public void DeleteUnit(uint unitId)
        {
            _units[unitId].Die();
        }
        public void DeleteUnit(Unit unit)
        {
            unit.Die();
            _units.Remove(unit.unitID);
        }

        public Dictionary<uint, Unit>.ValueCollection Values
        {
            get => _units.Values;
        }
        

        public IEnumerator<Unit> GetEnumerator()
        {
            foreach(var pair in _units)
            {
                yield return pair.Value;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}