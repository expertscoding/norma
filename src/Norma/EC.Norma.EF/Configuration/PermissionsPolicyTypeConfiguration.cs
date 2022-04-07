using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PermissionsPolicyTypeConfiguration : IEntityTypeConfiguration<PermissionsPolicy>
    {
        private string applicationId;

        public PermissionsPolicyTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }

        public void Configure(EntityTypeBuilder<PermissionsPolicy> builder)
        {
            builder.ToTable("PermissionsPolicies");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.IdPermission);
            builder.HasOne(a => a.Policy).WithMany().HasForeignKey(a => a.IdPolicy);

            builder.HasQueryFilter(a => a.Permission.Action.Module.Application.ApplicationId == applicationId);
        }
    }
}
