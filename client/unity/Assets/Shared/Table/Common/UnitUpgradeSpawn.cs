// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table.Common
{
    public class UnitUpgradeSpawn
    {
        [Key]
        public Ability Parent { get; set; }
    }
}