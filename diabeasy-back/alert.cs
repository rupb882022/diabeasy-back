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
    
    public partial class alert
    {
        public int id { get; set; }
        public Nullable<int> getting_user_id { get; set; }
        public Nullable<int> sendding_user_id { get; set; }
        public string content { get; set; }
        public Nullable<System.DateTime> date_time { get; set; }
        public Nullable<bool> active { get; set; }
    }
}
