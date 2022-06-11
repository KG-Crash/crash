// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table.Common
{
    public class SampleAttribute
    {
        [Key]
        public string GroupKey { get; set; }
        
        public string GroupField1 { get; set; }
        
        public string GroupField2 { get; set; }
        
        public int? GroupField3 { get; set; }
    }
}