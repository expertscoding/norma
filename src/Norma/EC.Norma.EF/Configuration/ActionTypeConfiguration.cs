using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.EF.Configuration
{
    public class ActionTypeConfiguration : IEntityTypeConfiguration<Action>
    {
        public void Configure(EntityTypeBuilder<Action> builder)
        {
            builder.ToTable("Actions");
            builder.HasKey(a => a.Id);


        }
    }
}
