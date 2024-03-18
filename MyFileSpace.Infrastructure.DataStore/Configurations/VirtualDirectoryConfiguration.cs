using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class VirtualDirectoryConfiguration : IEntityTypeConfiguration<VirtualDirectory>
    {
        public void Configure(EntityTypeBuilder<VirtualDirectory> builder)
        {
            //Primary key
            builder.HasKey(vd => vd.Id);

            //Properties
            builder.Property(vd => vd.VirtualPath)
                    .HasMaxLength(255)
                    .IsRequired();

            builder.Property(vd => vd.AccessLevel)
                .HasDefaultValue(AccessType.Private)
                .HasConversion<byte>()
                .IsRequired();

            //Foreign keys
            builder.HasOne(vd => vd.Owner)
                    .WithMany(u => u.Directories)
                    .HasForeignKey(vd => vd.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(true);

            builder.HasOne(vd => vd.ParentDirectory)
                    .WithMany(pd => pd.ChildDirectories)
                    .HasForeignKey(vd => vd.ParentDirectoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);
        }
    }
}
