// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class Projectile : Shared.Table.Common.Projectile
    {
        [Key]
        public int Id { get; set; }
        
        public bool Targeting { get; set; }
        
        public int Speed { get; set; }
    }
}