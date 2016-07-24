﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Models : DbContext
    {
        public Models()
            : base("name=Models")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<SensorType> SensorTypes { get; set; }
        public virtual DbSet<DBVersion> DBVersions { get; set; }
        public virtual DbSet<Mode> Modes { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<Zone> Zones { get; set; }
        public virtual DbSet<Reaction> Reactions { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<ConditionType> ConditionTypes { get; set; }
        public virtual DbSet<Command> Commands { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Parameter> Parameter { get; set; }
        public virtual DbSet<ParameterType> ParameterTypes { get; set; }
        public virtual DbSet<ComandParameterLink> ComandParameterLinks { get; set; }
        public virtual DbSet<DeviceParameterTypeLink> DeviceParameterTypeLinks { get; set; }
    }
}
