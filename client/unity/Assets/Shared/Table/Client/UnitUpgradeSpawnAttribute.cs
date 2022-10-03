// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class UnitUpgradeSpawnAttribute : Shared.Table.Common.UnitUpgradeSpawnAttribute
    {
        [Key]
        public Ability Upgrade { get; set; }
    }
}