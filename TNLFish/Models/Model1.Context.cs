﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TNLFish.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ShopCaCanhEntities : DbContext
    {
        public ShopCaCanhEntities()
            : base("name=ShopCaCanhEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CHITIETDONTHANG> CHITIETDONTHANGs { get; set; }
        public DbSet<DONDATHANG> DONDATHANGs { get; set; }
        public DbSet<dong_ca> dong_ca { get; set; }
        public DbSet<KHACHHANG> KHACHHANGs { get; set; }
        public DbSet<loai_ca> loai_ca { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<ADMIN> ADMINs { get; set; }
    }
}
