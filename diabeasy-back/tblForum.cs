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
    
    public partial class tblForum
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblForum()
        {
            this.tblForum1 = new HashSet<tblForum>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> date_time { get; set; }
        public string subject { get; set; }
        public string value { get; set; }
        public Nullable<int> Patients_id { get; set; }
        public Nullable<int> Doctor_id { get; set; }
        public Nullable<int> Id_Continue_comment { get; set; }
    
        public virtual tblDoctor tblDoctor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblForum> tblForum1 { get; set; }
        public virtual tblForum tblForum2 { get; set; }
        public virtual tblPatients tblPatients { get; set; }
    }
}
