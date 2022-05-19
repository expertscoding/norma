using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EC.Norma.EF.Configuration
{
    public class ActionRequirementTypeConfiguration : IEntityTypeConfiguration<ActionRequirement>
    {
        private string applicationKey;

        public ActionRequirementTypeConfiguration(string applicationKey)
        {
            this.applicationKey = applicationKey;
        }

        public void Configure(EntityTypeBuilder<ActionRequirement> builder)
        {
            builder.ToTable("ActionsRequirements");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Action).WithMany(b=>b.ActionRequirements).HasForeignKey(a => a.IdAction);
            builder.HasOne(a => a.Requirement).WithMany().HasForeignKey(a => a.IdRequirement);

        }
    }
}
