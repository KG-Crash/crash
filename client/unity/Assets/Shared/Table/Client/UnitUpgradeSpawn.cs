// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class UnitUpgradeSpawn : Shared.Table.Common.UnitUpgradeSpawn
    {
        
        public int Unit { get; set; }
        
        public int Count { get; set; }
        
        public int CycleSec { get; set; }
    }
}