using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class ResourceTypeConfiguration : IEntityTypeConfiguration<Resource>
    {
        private string applicationKey;

        public ResourceTypeConfiguration(string applicationKey)
        {
            this.applicationKey = applicationKey;
        }

        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.ToTable("Resources");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Module).WithMany().HasForeignKey(a => a.IdModule);

            builder.HasQueryFilter(a => a.Module.Application.Key == applicationKey);
        }
    }
}
