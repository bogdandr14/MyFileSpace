﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class AccessKeyConfiguration : IEntityTypeConfiguration<AccessKey>
    {
        public void Configure(EntityTypeBuilder<AccessKey> builder)
        {
            //Primary key
            builder.HasKey(ak => ak.Id);

            //Properties
            builder.Property(ak => ak.Key)
                .HasMaxLength(255)
                .IsFixedLength(true)
                .IsRequired();

            builder.Property(ak => ak.ExpiresAt)
                .HasDefaultValue(DateTime.UtcNow.AddDays(2))
                .IsRequired();
        }
    }
}