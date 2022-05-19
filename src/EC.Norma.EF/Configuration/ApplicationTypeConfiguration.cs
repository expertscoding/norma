using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class ApplicationTypeConfiguration : IEntityTypeConfiguration<Application>
    {
        private readonly string applicationKey;

        public ApplicationTypeConfiguration(string applicationKey)
        {
            this.applicationKey = applicationKey;
        }

        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("Applications");
            builder.HasKey(a => a.Id);

            builder.HasQueryFilter(a => a.Key == applicationKey);
        }
    }
}
