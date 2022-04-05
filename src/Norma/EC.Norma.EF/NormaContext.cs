using EC.Norma.Entities;
using EC.Norma.EF.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Proxies.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF
{
    public class NormaContext : DbContext
    {
        readonly ILoggerFactory loggerFactory;

        public NormaContext(ILoggerFactory loggerFactory = null, DbContextOptions<NormaContext> options = null) : base(options)
        {   
            this.loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (loggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }


            if (optionsBuilder.Options.FindExtension<ProxiesOptionsExtension>() == null)
            {
                optionsBuilder.UseLazyLoadingProxies(true);
            }

            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<Action> Actions { get; set; }

        public virtual DbSet<ActionsPolicy> ActionsPolicies { get; set; }

        public virtual DbSet<Assignment> Assignments { get; set; }

        public virtual DbSet<Permission> Permissions { get; set; }

        public virtual DbSet<PermissionsPolicy> PermissionsPolicies { get; set; }

        public virtual DbSet<Policy> Policies { get; set; }

        public virtual DbSet<Profile> Profiles { get; set; }

        public virtual DbSet<Resource> Resources { get; set; }

        public virtual DbSet<PriorityGroup> PriorityGroups { get; set; }

        public virtual DbSet<PolicyPriorityGroup> PoliciesPriorityGroups { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("norma");

            modelBuilder.ApplyConfiguration(new ActionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActionsPolicyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionsPolicyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PolicyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PriorityGroupTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PolicyPriorityGroupTypeConfiguration());

        }
    }
}
