using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Configurations
{
    internal class UserFileAccessConfiguration : IEntityTypeConfiguration<UserFileAccess>
    {
        public void Configure(EntityTypeBuilder<UserFileAccess> builder)
        {
            //Primary key
            builder.HasKey(ufa => new { ufa.FileId, ufa.AllowedUserId });

            //Properties
            builder.Property(ufa => ufa.FileId)
                .IsRequired();

            builder.Property(ufa => ufa.AllowedUserId)
                .IsRequired();

            //Foreign keys
            builder.HasOne(ufa => ufa.AllowedUser)
                    .WithMany(u => u.AllowedFiles)
                    .HasForeignKey(ufa => ufa.AllowedUserId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ufa => ufa.File)
                    .WithMany(sf => sf.AllowedUsers)
                    .HasForeignKey(ufa => ufa.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
