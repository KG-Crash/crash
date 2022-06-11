// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table.Common
{
    public class Sample2
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
    }
}