using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF.Configuration
{
    public class ModuleTypeConfiguration : IEntityTypeConfiguration<Module>
    {
        private string applicationId;

        public ModuleTypeConfiguration(string applicationId)
        {
            this.applicationId = applicationId;
        }

        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("Modules");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Application).WithMany().HasForeignKey(a => a.IdApplication);

            builder.HasQueryFilter(a => a.Application.ApplicationId == applicationId);
        }
    }
}
