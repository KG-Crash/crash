// DO NOT MODIFY

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class SampleAttribute
    {
        [Key]
        public string GroupKey { get; set; }
        
        public string GroupField1 { get; set; }
        
        public string GroupField2 { get; set; }
        
        public int? GroupField3 { get; set; }
    }

    public class Sample
    {
        [Key]
        public string Parent { get; set; }
        
        public string MemberField1 { get; set; }
        
        public string MemberField2 { get; set; }
        
        public string MemberField3 { get; set; }
    }

    public class Sample2
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
    }

    public class Unit
    {
        [Key]
        public int Id { get; set; }
        
        public int Hp { get; set; }
        
        public int Damage { get; set; }
        
        public int Armor { get; set; }
        
        public int AttackRange { get; set; }
        
        public int AttackSpeed { get; set; }
        
        public int Speed { get; set; }
        
        public UnitSize Size { get; set; }
        
        public UnitType Type { get; set; }
        
        public bool Controllable { get; set; }
        
        public int Width { get; set; }
        
        public int Height { get; set; }
        
        public bool Flyable { get; set; }
    }

    public class UnitUpgradeAbilityAttribute
    {
        [Key]
        public Ability Upgrade { get; set; }
    }

    public class UnitUpgradeAbility
    {
        [Key]
        public Ability Parent { get; set; }
        
        public int Unit { get; set; }
        
        public Dictionary<StatType, int> Additional { get; set; }
    }

    public class Skill
    {
        [Key]
        public int Id { get; set; }
        
        public int Unit { get; set; }
        
        public Ability Condition { get; set; }
    }

    public class Projectile
    {
        [Key]
        public int Id { get; set; }
        
        public bool Targeting { get; set; }
        
        public int Speed { get; set; }
    }

    public class UnitUpgradeCost
    {
        [Key]
        public Ability Upgrade { get; set; }
        
        public int Time { get; set; }
    }

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