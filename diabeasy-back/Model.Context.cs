﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class diabeasyDBContext : DbContext
    {
        public diabeasyDBContext()
            : base("name=diabeasyDBContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<tblATE_Ingredients> tblATE_Ingredients { get; set; }
        public virtual DbSet<tblATE_Recipes> tblATE_Recipes { get; set; }
        public virtual DbSet<tblBelong> tblBelong { get; set; }
        public virtual DbSet<tblCategory> tblCategory { get; set; }
        public virtual DbSet<tblConsistOf> tblConsistOf { get; set; }
        public virtual DbSet<tblDoctor> tblDoctor { get; set; }
        public virtual DbSet<tblExceptionalEvent> tblExceptionalEvent { get; set; }
        public virtual DbSet<tblForum> tblForum { get; set; }
        public virtual DbSet<tblGroupType> tblGroupType { get; set; }
        public virtual DbSet<tblHistorylog> tblHistorylog { get; set; }
        public virtual DbSet<tblIngredients> tblIngredients { get; set; }
        public virtual DbSet<tblInsulinType> tblInsulinType { get; set; }
        public virtual DbSet<tblPatientData> tblPatientData { get; set; }
        public virtual DbSet<tblPatients> tblPatients { get; set; }
        public virtual DbSet<tblPrescriptions> tblPrescriptions { get; set; }
        public virtual DbSet<tblRecipes> tblRecipes { get; set; }
        public virtual DbSet<tblUnitOfMeasure> tblUnitOfMeasure { get; set; }
    }
}
