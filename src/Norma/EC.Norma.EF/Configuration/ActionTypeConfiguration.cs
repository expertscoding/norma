using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF.Configuration
{
    public class ActionTypeConfiguration : IEntityTypeConfiguration<Action>
    {
        private string applicationId { get; }

        public ActionTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }       

        public void Configure(EntityTypeBuilder<Action> builder)
        {
            builder.ToTable("Actions");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Module).WithMany().HasForeignKey(a => a.IdModule);

            builder.HasQueryFilter(a => a.Module.Application.ApplicationId == applicationId);
        }
    }
}
