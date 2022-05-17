using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EC.Norma.EF.Configuration
{
    public class RequirementApplicationTypeConfiguration : IEntityTypeConfiguration<RequirementApplication>
    {
        private string applicationKey;

        public RequirementApplicationTypeConfiguration(string applicationKey )
        {
            this.applicationKey = applicationKey;
        }

        public void Configure(EntityTypeBuilder<RequirementApplication> builder)
        {
            builder.ToTable("RequirementsApplications");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Application).WithMany().HasForeignKey(a => a.IdApplication);
            builder.HasOne(a => a.Requirement).WithMany(b => b.RequirementsApplications).HasForeignKey(a => a.IdRequirement);

            builder.HasQueryFilter(a => a.Application.Key == applicationKey);
        }
    }
}
