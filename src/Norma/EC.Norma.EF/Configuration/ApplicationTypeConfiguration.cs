using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF.Configuration
{
    public class ApplicationTypeConfiguration : IEntityTypeConfiguration<Application>
    {
        private string applicationId;

        public ApplicationTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }

        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("Applications");
            builder.HasKey(a => a.Id);

            builder.HasQueryFilter(a => a.ApplicationId == applicationId);
        }
    }
}
