//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace diabeasy_back
{
    using System;
    using System.Collections.Generic;
    
    public partial class tempData
    {
        public int ID { get; set; }
        public string weekday { get; set; }
        public string dayTime { get; set; }
        public Nullable<int> blood_sugar_level { get; set; }
        public Nullable<double> value_of_ingection { get; set; }
        public Nullable<double> totalCarbs { get; set; }
        public Nullable<bool> good { get; set; }
    }
}