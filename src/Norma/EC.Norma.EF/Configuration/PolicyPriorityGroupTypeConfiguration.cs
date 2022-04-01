using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class PolicyPriorityGroupTypeConfiguration : IEntityTypeConfiguration<PolicyPriorityGroup>
    {
        public void Configure(EntityTypeBuilder<PolicyPriorityGroup> builder)
        {
            builder.ToTable("PoliciesPriorityGroups");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.PriorityGroup).WithMany().HasForeignKey(a => a.IdPriorityGroup);
            builder.HasOne(a => a.Policy).WithMany().HasForeignKey(a => a.IdPolicy);
        }
    }
}
