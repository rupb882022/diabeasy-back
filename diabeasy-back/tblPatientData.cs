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
    
    public partial class tblPatientData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPatientData()
        {
            this.tblATE_Ingredients = new HashSet<tblATE_Ingredients>();
            this.tblATE_Recipes = new HashSet<tblATE_Recipes>();
            this.tblExceptionalEvent = new HashSet<tblExceptionalEvent>();
        }
    
        public System.DateTime date_time { get; set; }
        public Nullable<int> blood_sugar_level { get; set; }
        public string injectionType { get; set; }
        public Nullable<double> system_recommendations { get; set; }
        public Nullable<double> value_of_ingection { get; set; }
        public string injection_site { get; set; }
        public int Patients_id { get; set; }
        public Nullable<double> totalCarbs { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblATE_Ingredients> tblATE_Ingredients { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblATE_Recipes> tblATE_Recipes { get; set; }
        public virtual tblPatients tblPatients { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblExceptionalEvent> tblExceptionalEvent { get; set; }
    }
}
