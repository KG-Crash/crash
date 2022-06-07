﻿using Shared;
using Shared.Table;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Player
    {
        public delegate void FinishUpgradeHandler(Ability ability);
        public delegate void AttackTargetChangedHandler(Player attacker, Player target);
        public delegate void LevelChangedHandler(Player player, uint level);

        public event FinishUpgradeHandler OnFinishUpgrade;
        public event AttackTargetChangedHandler OnAttackTargetChanged;
        public event LevelChangedHandler OnLevelChanged;

        public int id { get; private set; }
        public int spawnIndex { get; private set; }

        private Player _target;
        public Player target
        {
            get => _target;
            set
            {
                if (_target != value)
                    OnAttackTargetChanged?.Invoke(this, value);

                _target = value;
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
                OnLevelChanged?.Invoke(this, _level);
            }
        }

        public Upgrade upgrade { get; private set; }

        #region Upgrade
        /// <summary>
        /// 누적되는 업그레이드 상태
        /// </summary>
        public Dictionary<Advanced, int> advanced { get; private set; } = new Dictionary<Advanced, int>();
        #endregion

        /// <summary>
        /// 보유 유닛
        /// </summary>
        public UnitCollection units;

        public Team team { get; private set; }

        public Player(int id, Team team, int spawnIndex)
        {
            units = new UnitCollection(this);
            this.id = id;
            this.upgrade = new Upgrade(this);
            this.team = team;
            this.spawnIndex = spawnIndex;
        }

        public Dictionary<StatType, int> AdditionalStat(int unitID)
        {
            return Shared.Table.Table
                    .From<TableUnitUpgradeAbility>()
                    .Where(x => upgrade.abilities.HasFlag(x.Key))
                    .SelectMany(x => x.Value)
                    .Where(x => x.Unit == unitID)
                    .SelectMany(x => x.Additional)
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Sum(x => x.Value));
        }

        public override string ToString()
        {
            return $"{base.ToString()}(playerID={id}, teamID={team.id})";
        }
    }
}
