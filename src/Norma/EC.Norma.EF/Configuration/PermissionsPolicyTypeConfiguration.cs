using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PermissionsPolicyTypeConfiguration : IEntityTypeConfiguration<PermissionsPolicy>
    {
        public void Configure(EntityTypeBuilder<PermissionsPolicy> builder)
        {
            builder.ToTable("PermissionsPolicies");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.IdPermission);
            builder.HasOne(a => a.Policy).WithMany().HasForeignKey(a => a.IdPolicy);
        }
    }
}
