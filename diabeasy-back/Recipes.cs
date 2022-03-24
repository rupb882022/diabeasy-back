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
    
    public partial class Recipes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Recipes()
        {
            this.tblATE_Recipes = new HashSet<tblATE_Recipes>();
            this.tblBelongToRecipe = new HashSet<tblBelongToRecipe>();
            this.tblConsistOf = new HashSet<tblConsistOf>();
            this.tblCategory = new HashSet<tblCategory>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public Nullable<double> totalCarbohydrates { get; set; }
        public Nullable<double> totalsugars { get; set; }
        public Nullable<double> totalWeigthInGrams { get; set; }
        public string cookingMethod { get; set; }
        public Nullable<int> addByUserId { get; set; }
    
        public virtual tblPatients tblPatients { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblATE_Recipes> tblATE_Recipes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBelongToRecipe> tblBelongToRecipe { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblConsistOf> tblConsistOf { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCategory> tblCategory { get; set; }
    }
}