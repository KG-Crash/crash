using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    [Table("json/LevelExp.json")]
    public partial class TableLevelExp : BaseDict<int, LevelExp>
    { }

    [Table("json/SampleAttribute.json")]
    public partial class TableSampleAttribute : BaseDict<string, SampleAttribute>
    { }

    [Table("json/Sample.json")]
    public partial class TableSample : BaseDict<string, List<Sample>>
    { }

    [Table("json/Sample2.json")]
    public partial class TableSample2 : BaseDict<int, Sample2>
    { }

    [Table("json/Unit.json")]
    public partial class TableUnit : BaseDict<int, Unit>
    { }

    [Table("json/UnitUpgradeAbilityAttribute.json")]
    public partial class TableUnitUpgradeAbilityAttribute : BaseDict<Ability, UnitUpgradeAbilityAttribute>
    { }

    [Table("json/UnitUpgradeAbility.json")]
    public partial class TableUnitUpgradeAbility : BaseDict<Ability, List<UnitUpgradeAbility>>
    { }

    [Table("json/Skill.json")]
    public partial class TableSkill : BaseDict<int, Skill>
    { }

    [Table("json/Projectile.json")]
    public partial class TableProjectile : BaseDict<int, Projectile>
    { }

    [Table("json/UnitUpgradeCost.json")]
    public partial class TableUnitUpgradeCost : BaseDict<Ability, UnitUpgradeCost>
    { }
}