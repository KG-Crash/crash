// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class Unit : Shared.Table.Common.Unit
    {
        [Key]
        public int Id { get; set; }
        
        public int Hp { get; set; }
        
        public int Damage { get; set; }
        
        public int Armor { get; set; }
        
        public int VisibleRange { get; set; }
        
        public int AttackRange { get; set; }
        
        public int AttackSpeed { get; set; }
        
        public int Speed { get; set; }
        
        public UnitSize Size { get; set; }
        
        public UnitType Type { get; set; }
        
        public bool Controllable { get; set; }
        
        public int Width { get; set; }
        
        public int Height { get; set; }
        
        public bool Flyable { get; set; }
        
        public int KillScore { get; set; }
        
        public int ProjectileID { get; set; }
        
        public AttackType AttackType { get; set; }
    }
}