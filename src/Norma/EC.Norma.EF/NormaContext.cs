using EC.Norma.Entities;
using EC.Norma.EF.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using EC.Norma.Options;
using Microsoft.Extensions.Options;

namespace EC.Norma.EF
{
    public class NormaContext : DbContext
    {
        readonly ILoggerFactory loggerFactory;
        readonly NormaOptions options;

        public NormaContext(ILoggerFactory loggerFactory = null, DbContextOptions<NormaContext> options = null, IOptions<NormaOptions> normaOptions = null) : base(options)
        {   
            this.loggerFactory = loggerFactory;
            this.options = normaOptions?.Value;
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

        public virtual DbSet<Entities.Action> Actions { get; set; }

        public virtual DbSet<ActionsPolicy> ActionsPolicies { get; set; }

        public virtual DbSet<Assignment> Assignments { get; set; }

        public virtual DbSet<Permission> Permissions { get; set; }

        public virtual DbSet<PermissionsPolicy> PermissionsPolicies { get; set; }

        public virtual DbSet<Policy> Policies { get; set; }

        public virtual DbSet<Profile> Profiles { get; set; }

        public virtual DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("norma");

            modelBuilder.ApplyConfiguration(new ActionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActionsPolicyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionsPolicyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PolicyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceTypeConfiguration());

        }

        //public override int SaveChanges()
        //{
        //    var applicationId = options.ApplicationId;
        //    var changes = SetApplication(applicationId);
        //    var returnValue = base.SaveChanges();

        //    return returnValue;
        //}

        //private List<DbChange> SetApplication(string applicationId)
        //{
        //    var changes = new List<DbChange>();
           
        //    ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToList().ForEach(
        //        e =>
        //        {
        //            if (e.Entity is ITenantEntity tenantEntity)
        //            {
        //                tenantEntity.IdTenant = currentTenantId;
        //            }
        //            //if (e.Entity is DomainEntity)
        //            //{
        //            //    if (e.Entity is BaseEntity baseEntity)
        //            //    {
        //            //        baseEntity.DInserted = current;
        //            //        baseEntity.IdSession_Ins = idSession;
        //            //    }
        //            //    changes.Add(BuildChange(e));
        //            //}
        //        });
        //    return changes;
        //}
    }
}
