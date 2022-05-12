using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF.Configuration
{
    public class AssignmentTypeConfiguration : IEntityTypeConfiguration<Assignment>
    {
        private string applicationKey;

        public AssignmentTypeConfiguration(string applicationKey)
        {
            this.applicationKey = applicationKey;
        }

        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.IdPermission);
            builder.HasOne(a => a.Profile).WithMany().HasForeignKey(a => a.IdProfile);

            builder.HasQueryFilter(a => a.Permission.Resource.Module.Application.Key == applicationKey);
        }
    }
}
