using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF.Configuration
{
    public class AssignmentTypeConfiguration : IEntityTypeConfiguration<Assignment>
    {
        private string applicationId;

        public AssignmentTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }

        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.IdPermission);
            builder.HasOne(a => a.Profile).WithMany().HasForeignKey(a => a.IdProfile);

            builder.HasQueryFilter(a => a.Permission.Action.Module.Application.ApplicationId == applicationId);
            builder.HasQueryFilter(a => a.Permission.Resource.Module.Application.ApplicationId == applicationId);
        }
    }
}
