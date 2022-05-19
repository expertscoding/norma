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
        readonly string ApplicationKey;

        public NormaContext(IOptions<NormaOptions> normaOptions, ILoggerFactory loggerFactory = null, DbContextOptions<NormaContext> options = null) : base(options)
        {   
            this.loggerFactory = loggerFactory;
            this.normaOptions = normaOptions.Value;

            ApplicationKey = this.normaOptions.ApplicationKey;
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

            modelBuilder.ApplyConfiguration(new ActionTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new ActionRequirementTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new ApplicationTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new AssignmentTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new ModuleTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new PermissionRequirementTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new PermissionTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new RequirementTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceTypeConfiguration(ApplicationKey));
            modelBuilder.ApplyConfiguration(new PriorityGroupTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequirementPriorityGroupTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequirementApplicationTypeConfiguration(ApplicationKey));

        }

       
    }
}
