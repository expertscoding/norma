using EC.Norma.Entities;
using EC.Norma.EF.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies.Internal;
using Microsoft.Extensions.Logging;
using EC.Norma.Options;
using Microsoft.Extensions.Options;

namespace EC.Norma.EF
{
    public class NormaContext : DbContext
    {
        readonly ILoggerFactory loggerFactory;
        readonly NormaOptions normaOptions;
        readonly string ApplicationId;

        public NormaContext(IOptions<NormaOptions> normaOptions, ILoggerFactory loggerFactory = null, DbContextOptions<NormaContext> options = null) : base(options)
        {   
            this.loggerFactory = loggerFactory;
            this.normaOptions = normaOptions.Value;

            ApplicationId = this.normaOptions.ApplicationId;
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

        public virtual DbSet<ActionsRequirement> ActionsRequirements { get; set; }

        public virtual DbSet<Application> Applications { get; set; }

        public virtual DbSet<Assignment> Assignments { get; set; }

        public virtual DbSet<Module> Modules { get; set; }

        public virtual DbSet<Permission> Permissions { get; set; }

        public virtual DbSet<PermissionsRequirement> PermissionsRequirements { get; set; }

        public virtual DbSet<Requirement> Requirements { get; set; }

        public virtual DbSet<Profile> Profiles { get; set; }

        public virtual DbSet<Resource> Resources { get; set; }

        public virtual DbSet<PriorityGroup> PriorityGroups { get; set; }

        public virtual DbSet<RequirementPriorityGroup> RequirementsPriorityGroups { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("norma");

            modelBuilder.ApplyConfiguration(new ActionTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new ActionRequirementTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new ApplicationTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new AssignmentTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new ModuleTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new PermissionRequirementTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new PermissionTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new RequirementTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceTypeConfiguration(ApplicationId));
            modelBuilder.ApplyConfiguration(new PriorityGroupTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequirementPriorityGroupTypeConfiguration());

        }

       
    }
}
