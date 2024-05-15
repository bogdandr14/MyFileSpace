using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Configurations
{
    internal class UserAccessKeyConfiguration :
    IEntityTypeConfiguration<UserAccessKey>
    {
        public void Configure(EntityTypeBuilder<UserAccessKey> builder)
        {
            //Primary key
            builder.HasKey(uak => new { uak.UserId, uak.AccessKeyId });

            //Properties
            builder.Property(uak => uak.UserId)
                .IsRequired();

            builder.Property(uak => uak.Type)
                .IsRequired();

            builder.Property(uak => uak.AccessKeyId)
                .IsRequired();

            //Foreign keys
            builder.HasOne(uak => uak.User)
                    .WithMany(u => u.UserAccessKeys)
                    .HasForeignKey(uak => uak.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(uak => uak.AccessKey)
                    .WithOne(ak => ak.UserAccess)
                    .HasForeignKey<UserAccessKey>(uak => uak.AccessKeyId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
