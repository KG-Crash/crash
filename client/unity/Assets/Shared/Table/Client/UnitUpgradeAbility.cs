// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class UnitUpgradeAbility : Shared.Table.Common.UnitUpgradeAbility
    {
        
        public int Unit { get; set; }
        
        public Dictionary<StatType, int> Additional { get; set; }
    }
}