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
    
    public partial class tblATE_Recipes
    {
        public Nullable<bool> isFavorite { get; set; }
        public Nullable<bool> isGood { get; set; }
        public Nullable<double> amount { get; set; }
        public Nullable<int> UnitOfMeasure_id { get; set; }
        public int Patients_id { get; set; }
        public System.DateTime date_time { get; set; }
        public int Recipe_id { get; set; }
    
        public virtual Recipes Recipes { get; set; }
        public virtual tblUnitOfMeasure tblUnitOfMeasure { get; set; }
        public virtual tblPatientData tblPatientData { get; set; }
    }
}
