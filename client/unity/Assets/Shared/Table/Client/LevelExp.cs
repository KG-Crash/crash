// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table
{
    public class LevelExp : Shared.Table.Common.LevelExp
    {
        [Key]
        public int Level { get; set; }
        
        public int Exp { get; set; }
    }
}