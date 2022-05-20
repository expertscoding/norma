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
        private readonly ILoggerFactory loggerFactory;
        private readonly string applicationKey;

        public NormaContext(IOptions<NormaOptions> normaOptions, ILoggerFactory loggerFactory = null, DbContextOptions<NormaContext> options = null) : base(options)
        {   
            this.loggerFactory = loggerFactory;

            applicationKey = normaOptions.Value.ApplicationKey;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (loggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }


            if (optionsBuilder.Options.FindExtension<ProxiesOptionsExtension>() == null)
            {
                optionsBuilder.UseLazyLoadingProxies();
            }

            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<Action> Actions { get; set; }

        public virtual DbSet<ActionRequirement> ActionsRequirements { get; set; }

        public virtual DbSet<Application> Applications { get; set; }

        public virtual DbSet<Assignment> Assignments { get; set; }

        public virtual DbSet<Module> Modules { get; set; }

        public virtual DbSet<Permission> Permissions { get; set; }

        public virtual DbSet<PermissionRequirement> PermissionsRequirements { get; set; }

        public virtual DbSet<Requirement> Requirements { get; set; }

        public virtual DbSet<Profile> Profiles { get; set; }

        public virtual DbSet<Resource> Resources { get; set; }

        public virtual DbSet<PriorityGroup> PriorityGroups { get; set; }

        public virtual DbSet<RequirementPriorityGroup> RequirementsPriorityGroups { get; set; }

        public virtual DbSet<RequirementApplication> RequirementsApplications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("norma");

            modelBuilder.ApplyConfiguration(new ActionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActionRequirementTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationTypeConfiguration(applicationKey));
            modelBuilder.ApplyConfiguration(new AssignmentTypeConfiguration(applicationKey));
            modelBuilder.ApplyConfiguration(new ModuleTypeConfiguration(applicationKey));
            modelBuilder.ApplyConfiguration(new PermissionRequirementTypeConfiguration(applicationKey));
            modelBuilder.ApplyConfiguration(new PermissionTypeConfiguration(applicationKey));
            modelBuilder.ApplyConfiguration(new RequirementTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceTypeConfiguration(applicationKey));
            modelBuilder.ApplyConfiguration(new PriorityGroupTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequirementPriorityGroupTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequirementApplicationTypeConfiguration(applicationKey));

        }

       
    }
}
