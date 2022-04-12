using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PermissionRequirementTypeConfiguration : IEntityTypeConfiguration<PermissionsRequirement>
    {
        private string applicationId;

        public PermissionRequirementTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }

        public void Configure(EntityTypeBuilder<PermissionsRequirement> builder)
        {
            builder.ToTable("PermissionsRequirements");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.IdPermission);
            builder.HasOne(a => a.Requirement).WithMany().HasForeignKey(a => a.IdRequirement);

            builder.HasQueryFilter(a => a.Permission.Action.Module.Application.ApplicationId == applicationId);
        }
    }
}
