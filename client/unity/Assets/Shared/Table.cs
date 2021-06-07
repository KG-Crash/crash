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

    [Table("json/SampleAttribute.json")]
    public partial class TableSampleAttribute : BaseDict<string, SampleAttribute>
    { }

    [Table("json/Sample.json")]
    public partial class TableSample : BaseDict<string, List<Sample>>
    { }

    [Table("json/Sample2.json")]
    public partial class TableSample2 : BaseDict<int, Sample2>
    { }
}