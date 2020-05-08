using AtomStore.Data.EF.Extensions;
using AtomStore.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.EF.Configuration
{
    public class MessageConfiguration : DbEntityConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> entity)
        {
            entity.HasOne<AppUser>(a => a.AppUser).WithMany(d => d.Messages).HasForeignKey(d => d.UserId);
        }
    }
}
