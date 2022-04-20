using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF.Configuration
{
    public class ActionTypeConfiguration : IEntityTypeConfiguration<Action>
    {
        private string applicationKey { get; }

        public ActionTypeConfiguration(string applicationKey)
        {
            this.applicationKey = applicationKey;
        }       

        public void Configure(EntityTypeBuilder<Action> builder)
        {
            builder.ToTable("Actions");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Module).WithMany().HasForeignKey(a => a.IdModule);

            builder.HasQueryFilter(a => a.Module.Application.Key == applicationKey);
        }
    }
}
