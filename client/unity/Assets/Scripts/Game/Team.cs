using Shared;
using Shared.Table;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    #region Team
    public class Team
    {
        public Dictionary<uint, List<Player>> players { get; private set; } = new Dictionary<uint, List<Player>>();
    }
    #endregion

    #region Player
    public class Player
    {
        public interface IPlayerListener
        {
            void FinishUpgrade(Ability ability);
            void AttackTargetChanged(int playerId, int targetPlayerID);
            void PlayerLevelChanged(int playerID, int level);
        }
        
        public uint playerID { get; private set; }
        public uint teamID { get; private set; }
        public int spawnIndex { get; set; }
        
        public IPlayerListener listener { get; private set; }
        
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
        public UnitCollection units;

        public Player(uint playerID, uint teamID, IPlayerListener listener)
        {
            units = new UnitCollection(this);
            this.listener = listener;
            this.playerID = playerID;
            this.teamID = teamID;
        }

        public Dictionary<StatType, int> AdditionalStat(uint unitID)
        {
            return Shared.Table.Table
                    .From<TableUnitUpgradeAbility>()
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
        
        private Dictionary<Ability, float> _upgradeStartTime = new Dictionary<Ability, float>();

        public void StartUpgrade(Ability ability)
        {
            if (ability == Ability.NONE)
            {
                return;
            }
            
            if (!_upgradeStartTime.ContainsKey(ability))
            {
                _upgradeStartTime.Add(ability, UnityEngine.Time.time);   
                Debug.Log($"StartUpgrade({ability}), After {Table.From<TableUnitUpgradeCost>()[ability].Time}ms");
            }
        }

        public void UpdateUpgrade(float time)
        {
            // 임시 테스트 코드
            if (Input.GetKeyUp(KeyCode.Alpha1))
                StartUpgrade(Ability.UPGRADE_1);
            if (Input.GetKeyUp(KeyCode.Alpha2))
                StartUpgrade(Ability.UPGRADE_2);
            if (Input.GetKeyUp(KeyCode.Alpha3))
                StartUpgrade(Ability.UPGRADE_3);
            if (Input.GetKeyUp(KeyCode.Alpha4))
                StartUpgrade(Ability.UPGRADE_4);
            if (Input.GetKeyUp(KeyCode.Alpha5))
                StartUpgrade(Ability.UPGRADE_5);
            if (Input.GetKeyUp(KeyCode.Alpha6))
                StartUpgrade(Ability.UPGRADE_6);
            if (Input.GetKeyUp(KeyCode.Alpha7))
                StartUpgrade(Ability.UPGRADE_7);
            if (Input.GetKeyUp(KeyCode.Alpha8))
                StartUpgrade(Ability.UPGRADE_8);
            if (Input.GetKeyUp(KeyCode.Alpha9))
                StartUpgrade(Ability.UPGRADE_9);
            // 임시 테스트 코드
            
            List<Ability> completeList = new List<Ability>();
            
            foreach (var k in _upgradeStartTime.Keys)
            {
                if (_upgradeStartTime[k] + Table.From<TableUnitUpgradeCost>()[k].Time / 1000.0f < time)
                {
                    completeList.Add(k);
                }
            }

            foreach (var ability in completeList)
            {
                _upgradeStartTime.Remove(ability);
                SetAbilityFlag(ability);
                listener?.FinishUpgrade(ability);
            }
        }
    }
    #endregion

    #region UnitCollection
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
                unit.owner.units._units.Remove(unit.unitID);
            }

            unit.owner = this._owner;
            _units.Add(unit.unitID, unit);
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
            _units.Remove(unit.unitID);
        }

        public Dictionary<uint, Unit>.ValueCollection Values => _units.Values;


        public IEnumerator<Unit> GetEnumerator()
        {
            foreach(var pair in _units)
            {
                yield return pair.Value;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    #endregion
}