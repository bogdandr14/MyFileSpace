using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Configurations
{
    internal class LabelConfiguration : IEntityTypeConfiguration<Label>
    {
        public void Configure(EntityTypeBuilder<Label> builder)
        {
            //Primary key
            builder.HasKey(l => l.Id);

            //Properties
            builder.Property(l => l.Name)
                    .HasMaxLength(50)
                    .IsRequired();

            //Foreign keys
            builder.HasOne(l => l.Owner)
                    .WithMany(u => u.Labels)
                    .HasForeignKey(l => l.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
