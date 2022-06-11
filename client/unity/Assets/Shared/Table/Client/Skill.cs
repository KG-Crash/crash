// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class Skill : Shared.Table.Common.Skill
    {
        [Key]
        public int Id { get; set; }
        
        public int Unit { get; set; }
        
        public Ability Condition { get; set; }
    }
}