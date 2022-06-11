// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class UnitUpgradeCost : Shared.Table.Common.UnitUpgradeCost
    {
        [Key]
        public Ability Upgrade { get; set; }
        
        public int Time { get; set; }
    }
}