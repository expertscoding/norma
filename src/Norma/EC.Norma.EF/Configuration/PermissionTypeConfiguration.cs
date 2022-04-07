using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PermissionTypeConfiguration : IEntityTypeConfiguration<Permission>
    {
        private string applicationId;

        public PermissionTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }

        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Action).WithMany().HasForeignKey(a => a.IdAction);
            builder.HasOne(a => a.Resource).WithMany().HasForeignKey(a => a.IdResource);


            builder.HasQueryFilter(a => a.Action.Module.Application.ApplicationId == applicationId);

            builder.HasQueryFilter(a => a.Action.Module.Application.ApplicationId == applicationId);
            builder.HasQueryFilter(a => a.Resource.Module.Application.ApplicationId == applicationId);

        }
    
    }
}
