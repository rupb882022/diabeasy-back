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
    
    public partial class tblConsistOf
    {
        public Nullable<double> amount { get; set; }
        public int Recipes_id { get; set; }
        public int Ingredients_id { get; set; }
        public int UnitOfMeasure_id { get; set; }
    
        public virtual tblIngredients tblIngredients { get; set; }
        public virtual tblRecipes tblRecipes { get; set; }
        public virtual tblUnitOfMeasure tblUnitOfMeasure { get; set; }
    }
}
