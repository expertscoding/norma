using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EC.Norma.EF.Configuration
{
    public class ActionsPolicyTypeConfiguration : IEntityTypeConfiguration<ActionsPolicy>
    {
        public void Configure(EntityTypeBuilder<ActionsPolicy> builder)
        {
            builder.ToTable("ActionsPolicies");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Action).WithMany(b=>b.ActionPolicies).HasForeignKey(a => a.IdAction);
            builder.HasOne(a => a.Policy).WithMany().HasForeignKey(a => a.IdPolicy);
        }
    }
}
