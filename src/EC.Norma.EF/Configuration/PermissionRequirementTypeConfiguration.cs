using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PermissionRequirementTypeConfiguration : IEntityTypeConfiguration<PermissionRequirement>
    {
        private readonly string applicationKey;

        public PermissionRequirementTypeConfiguration(string applicationKey)
        {
            this.applicationKey = applicationKey;
        }

        public void Configure(EntityTypeBuilder<PermissionRequirement> builder)
        {
            builder.ToTable("PermissionsRequirements");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.IdPermission);
            builder.HasOne(a => a.Requirement).WithMany().HasForeignKey(a => a.IdRequirement);

            builder.HasQueryFilter(a => a.Permission.Resource.Module.Application.Key == applicationKey);
        }
    }
}
