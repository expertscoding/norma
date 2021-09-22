using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PolicyTypeConfiguration : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.ToTable("Policies");
            builder.HasKey(a => a.Id);


        }
    }
}
