// DO NOT MODIFY

using Newtonsoft.Json;
using Shared.Type;
using System;
using System.Collections.Generic;

namespace Shared.Table.Common
{
    public class Sample
    {
        [Key]
        public string Parent { get; set; }
        
        public string MemberField1 { get; set; }
        
        public string MemberField2 { get; set; }
        
        public string MemberField3 { get; set; }
    }
}