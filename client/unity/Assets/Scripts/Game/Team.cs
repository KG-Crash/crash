using Shared;
using Shared.Table;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    #region Team
    public class Team
    {
        public Dictionary<uint, List<Player>> players { get; private set; } = new Dictionary<uint, List<Player>>();

        public void AddPlayer(uint teamID, Player player)
        {
            List<Player> playerList = null;
            
            if (players.ContainsKey(teamID))
            {
                playerList = players[teamID];

                if (playerList == null)
                {
                    playerList = new List<Player>();
                    players[teamID] = playerList;
                }
            }
            else
            {                    
                playerList = new List<Player>();
                players.Add(teamID, playerList);
            }
            
            playerList.Add(player);
        }

        public Player GetPlayer(uint playerID)
        {
            var finedPlayer = players.
                SelectMany(x => x.Value).
                FirstOrDefault(x => x.playerID == playerID);

            if (finedPlayer == default(Player))
            {
                return null;                    
            }
            else
            {
                return finedPlayer;
            }
        }
    }
    #endregion
    
    #region Player
    public class Player
    {
        public interface IPlayerListener
        {
            void OnFinishUpgrade(Ability ability);
            void OnAttackTargetChanged(uint playerId, uint? targetPlayerID);
            void OnPlayerLevelChanged(uint playerID, uint level);
        }
        
        public uint playerID { get; private set; }
        public uint teamID { get; private set; }
        public int spawnIndex { get; set; }
        
        private uint? _targetPlayerID = null;
        public uint? targetPlayerID
        {
            get => _targetPlayerID;
            set
            {
                _targetPlayerID = value;
                listener?.OnAttackTargetChanged(playerID, _targetPlayerID);
            }
        }

        public uint _exp = 0;
        public uint exp
        {
            get => _exp;
            set
            {
                _exp = value;

                var levelExpTable = Table.From<TableLevelExp>();
                
                if (levelExpTable.ContainsKey((int)_level + 1))
                {
                    if ((int)_exp >= levelExpTable[(int)_level + 1].Exp)
                    {
                        level = _level + 1;
                    }
                }
            }
        }

        private uint _level = 1;
        public uint level
        {
            get => _level;
            set
            {
                _level = value;
                listener?.OnPlayerLevelChanged(playerID, _level);
            }
        }

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
                listener?.OnFinishUpgrade(ability);
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}(playerID={playerID}, teamID={teamID})";
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
            // unit.owner = null;
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