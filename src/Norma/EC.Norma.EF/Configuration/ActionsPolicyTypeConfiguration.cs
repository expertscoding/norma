using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EC.Norma.EF.Configuration
{
    public class ActionsPolicyTypeConfiguration : IEntityTypeConfiguration<ActionsPolicy>
    {
        private string applicationId;

        public ActionsPolicyTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }

        public void Configure(EntityTypeBuilder<ActionsPolicy> builder)
        {
            builder.ToTable("ActionsPolicies");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Action).WithMany(b=>b.ActionPolicies).HasForeignKey(a => a.IdAction);
            builder.HasOne(a => a.Policy).WithMany().HasForeignKey(a => a.IdPolicy);

            builder.HasQueryFilter(a => a.Action.Module.Application.ApplicationId == applicationId);
        }
    }
}
