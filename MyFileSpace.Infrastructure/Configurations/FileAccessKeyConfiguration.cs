using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Configurations
{
    internal class FileAccessKeyConfiguration : IEntityTypeConfiguration<FileAccessKey>
    {
        public void Configure(EntityTypeBuilder<FileAccessKey> builder)
        {
            //Primary key
            builder.HasKey(fak => new { fak.FileId, fak.AccessKeyId });

            //Properties
            builder.Property(fak => fak.FileId)
                .IsRequired();

            builder.Property(fak => fak.AccessKeyId)
                .IsRequired();

            //Foreign keys
            builder.HasOne(fak => fak.AccessibleFile)
                    .WithOne(sf => sf.FileAccessKey)
                    .HasForeignKey<FileAccessKey>(fak => fak.FileId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fak => fak.AccessKey)
                    .WithOne(ak => ak.FileAccess)
                    .HasForeignKey<FileAccessKey>(fak => fak.AccessKeyId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
