using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class FavoriteFileConfiguration : IEntityTypeConfiguration<FavoriteFile>
    {
        public void Configure(EntityTypeBuilder<FavoriteFile> builder)
        {
            //Primary key
            builder.HasKey(ff => new { ff.UserId, ff.FileId });

            //Properties
            builder.Property(ff => ff.UserId)
                .IsRequired();

            builder.Property(ff => ff.FileId)
                .IsRequired();

            //Foreign keys
            builder.HasOne(ff => ff.User)
                    .WithMany(u => u.FavoriteFiles)
                    .HasForeignKey(ff => ff.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ff => ff.File)
                    .WithMany(sf => sf.UsersFavorite)
                    .HasForeignKey(ff => ff.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
