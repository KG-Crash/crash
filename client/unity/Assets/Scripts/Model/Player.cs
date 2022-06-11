using Shared.Table;
using Shared.Type;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Player
    {
        public interface Listener : Unit.Listener
        {
            void OnFinishUpgrade(Ability ability);
            void OnAttackTargetChanged(Player attacker, Player target);
            void OnPlayerLevelChanged(Player player, uint level);
        }

        public int id { get; private set; }
        public int spawnIndex { get; private set; }

        private Player _target;
        public Player target
        {
            get => _target;
            set
            {
                if (_target != value)
                    listener?.OnAttackTargetChanged(this, value);

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
                listener?.OnPlayerLevelChanged(this, _level);
            }
        }

        public Listener listener { get; private set; }

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

        public Player(int id, Team team, int spawnIndex, Listener listener)
        {
            units = new UnitCollection(this, listener);
            this.listener = listener;
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
