﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //Primary key
            builder.HasKey(u => u.Id);
            builder.HasAlternateKey(u => u.TagName);

            //Index
            builder.HasIndex(u => u.TagName).IsUnique();

            //Properties
            builder.Property(u => u.TagName)
                    .HasMaxLength(50)
                    .IsRequired();

            builder.Property(u => u.Email)
                    .HasMaxLength(321)
                    .IsRequired();

            builder.Property(u => u.Password)
                    .HasMaxLength(255)
                    .IsRequired();

            builder.Property(u => u.Role)
                .HasDefaultValue(RoleType.Customer)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(u => u.LastPasswordChange)
                .HasDefaultValue(DateTime.MinValue)
                .ValueGeneratedOnUpdateSometimes()
                .IsRequired();

            builder.Property(u => u.Salt)
                    .HasMaxLength(64)
                    .IsRequired();
        }
    }
}
