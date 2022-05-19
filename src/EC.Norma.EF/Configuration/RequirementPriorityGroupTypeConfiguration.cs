using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class RequirementPriorityGroupTypeConfiguration : IEntityTypeConfiguration<RequirementPriorityGroup>
    {
        public void Configure(EntityTypeBuilder<RequirementPriorityGroup> builder)
        {
            builder.ToTable("RequirementsPriorityGroups");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.PriorityGroup).WithMany().HasForeignKey(a => a.IdPriorityGroup);
            builder.HasOne(a => a.Requirement).WithMany(b => b.RequirementsPriorityGroups).HasForeignKey(a => a.IdRequirement);
        }
    }
}
