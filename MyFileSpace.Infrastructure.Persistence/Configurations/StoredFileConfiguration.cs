using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
    {
        public void Configure(EntityTypeBuilder<StoredFile> builder)
        {
            //Primary key
            builder.HasKey(sf => sf.Id);

            //Properties
            builder.Property(sf => sf.Name)
                    .HasMaxLength(255)
                    .IsRequired();

            builder.Property(sf => sf.AccessLevel)
                .HasDefaultValue(AccessType.Private)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(sf => sf.SizeInBytes)
                .IsRequired();

            builder.Property(sf => sf.IsDeleted)
                .IsRequired();

            builder.Property(sf => sf.ContentType)
                .IsRequired();

            //Foreign keys
            builder.HasOne(sf => sf.Owner)
                    .WithMany(u => u.Files)
                    .HasForeignKey(sf => sf.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sf => sf.Directory)
                    .WithMany(sf => sf.FilesInDirectory)
                    .HasForeignKey(sf => sf.DirectorId)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
