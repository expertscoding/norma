using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PriorityGroupTypeConfiguration : IEntityTypeConfiguration<PriorityGroup>
    {
        public void Configure(EntityTypeBuilder<PriorityGroup> builder)
        {
            builder.ToTable("PriorityGroups");
            builder.HasKey(a => a.Id);


        }
    }
}
