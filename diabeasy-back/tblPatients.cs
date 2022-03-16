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
    
    public partial class tblPatients
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPatients()
        {
            this.tblForum = new HashSet<tblForum>();
            this.tblHistorylog = new HashSet<tblHistorylog>();
            this.tblIngredients = new HashSet<tblIngredients>();
            this.tblPatientData = new HashSet<tblPatientData>();
            this.tblPrescriptions = new HashSet<tblPrescriptions>();
            this.tblRecipes = new HashSet<tblRecipes>();
        }
    
        public int id { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public Nullable<System.DateTime> birthdate { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public string profileimage { get; set; }
        public Nullable<double> height { get; set; }
        public Nullable<double> weight { get; set; }
        public string injectionType { get; set; }
        public Nullable<int> assistant_phone { get; set; }
        public Nullable<double> long_boluos_value { get; set; }
        public Nullable<double> fix_injection_ratio { get; set; }
        public Nullable<double> food_injection_ratio { get; set; }
        public Nullable<int> InsulinType_id { get; set; }
        public Nullable<int> GroupType_id { get; set; }
        public Nullable<int> Doctor_id { get; set; }
        public Nullable<int> InsulinType_long_id { get; set; }
    
        public virtual tblDoctor tblDoctor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblForum> tblForum { get; set; }
        public virtual tblGroupType tblGroupType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblHistorylog> tblHistorylog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblIngredients> tblIngredients { get; set; }
        public virtual tblInsulinType tblInsulinType { get; set; }
        public virtual tblInsulinType tblInsulinType1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPatientData> tblPatientData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPrescriptions> tblPrescriptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRecipes> tblRecipes { get; set; }
    }
}
