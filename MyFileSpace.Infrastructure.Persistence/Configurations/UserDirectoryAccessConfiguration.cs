using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class UserDirectoryAccessConfiguration : IEntityTypeConfiguration<UserDirectoryAccess>
    {
        public void Configure(EntityTypeBuilder<UserDirectoryAccess> builder)
        {
            //Primary key
            builder.HasKey(da => new { da.DirectoryId, da.AllowedUserId });

            //Properties
            builder.Property(da => da.DirectoryId)
                .IsRequired();

            builder.Property(da => da.AllowedUserId)
                .IsRequired();

            //Foreign keys
            builder.HasOne(da => da.AllowedUser)
                    .WithMany(u => u.AllowedDirectories)
                    .HasForeignKey(da => da.AllowedUserId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(da => da.Directory)
                    .WithMany(vd => vd.AllowedUsers)
                    .HasForeignKey(da => da.DirectoryId)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
