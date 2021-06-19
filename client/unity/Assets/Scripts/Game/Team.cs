using Shared;
using Shared.Table;
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
        public Dictionary<uint, Unit> units { get; private set; } = new Dictionary<uint, Unit>();

        public void AddUnits(IEnumerable<Unit> otherUnits)
        {            
            units = 
                units.Union(otherUnits.ToDictionary(u => u.unitID, u => u)).
                ToDictionary(u => u.Key, u => u.Value);
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
}