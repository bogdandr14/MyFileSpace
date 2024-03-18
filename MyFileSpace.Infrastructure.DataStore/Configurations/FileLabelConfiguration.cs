using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class FileLabelConfiguration : IEntityTypeConfiguration<FileLabel>
    {
        public void Configure(EntityTypeBuilder<FileLabel> builder)
        {
            //Primary key
            builder.HasKey(fl => new { fl.LabelId, fl.FileId });

            //Properties
            builder.Property(fl => fl.LabelId)
                .IsRequired();

            builder.Property(fl => fl.FileId)
                .IsRequired();

            //Foreign keys
            builder.HasOne(fl => fl.Label)
                    .WithMany(l => l.FilesWithLabel)
                    .HasForeignKey(fl => fl.LabelId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fl => fl.File)
                    .WithMany(sf => sf.Labels)
                    .HasForeignKey(fl => fl.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
