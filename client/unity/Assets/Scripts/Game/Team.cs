using Shared;
using System.Collections.Generic;

namespace Game
{
    public class Team
    {
        public Dictionary<uint, List<Player>> Players { get; private set; } = new Dictionary<uint, List<Player>>();
    }

    public class Player
    {
        #region Upgrade
        /// <summary>
        /// 업그레이드 상태
        /// </summary>
        public Ability Abilities { get; private set; } = Ability.NONE;
        
        /// <summary>
        /// 누적되는 업그레이드 상태
        /// </summary>
        public Dictionary<Advanced, int> Advanced { get; private set; } = new Dictionary<Advanced, int>();
        #endregion

        /// <summary>
        /// 보유 유닛
        /// </summary>
        public Dictionary<uint, Unit> Units { get; private set; } = new Dictionary<uint, Unit>();
    }
}