using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class DirectoryAccessKeyConfiguration : IEntityTypeConfiguration<DirectoryAccessKey>
    {
        public void Configure(EntityTypeBuilder<DirectoryAccessKey> builder)
        {
            //Primary key
            builder.HasKey(dak => new { dak.DirectoryId, dak.AccessKeyId });

            //Properties
            builder.Property(dak => dak.DirectoryId)
                .IsRequired();

            builder.Property(dak => dak.AccessKeyId)
                .IsRequired();

            //Foreign keys
            builder.HasOne(dak => dak.AccessibleDirectory)
                    .WithOne(vd => vd.DirectoryAccessKey)
                    .HasForeignKey<DirectoryAccessKey>(dak => dak.DirectoryId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dak => dak.AccessKey)
                    .WithOne(ak => ak.DirectoryAccess)
                    .HasForeignKey<DirectoryAccessKey>(dak => dak.AccessKeyId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
