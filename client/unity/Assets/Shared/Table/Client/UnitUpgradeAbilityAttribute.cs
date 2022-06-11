// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class UnitUpgradeAbilityAttribute : Shared.Table.Common.UnitUpgradeAbilityAttribute
    {
        [Key]
        public Ability Upgrade { get; set; }
    }
}