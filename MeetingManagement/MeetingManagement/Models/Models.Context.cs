﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeetingManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SEP24Team7Entities : DbContext
    {
        public SEP24Team7Entities()
            : base("name=SEP24Team7Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<ATTACHMENT> ATTACHMENTs { get; set; }
        public virtual DbSet<CATEGORY> CATEGORies { get; set; }
        public virtual DbSet<MEETING> MEETINGs { get; set; }
        public virtual DbSet<MEETING_STATUS> MEETING_STATUS { get; set; }
        public virtual DbSet<MEMBER> MEMBERs { get; set; }
        public virtual DbSet<OTHER_ACCOUNT> OTHER_ACCOUNTs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TASK> TASKs { get; set; }
    }
}
