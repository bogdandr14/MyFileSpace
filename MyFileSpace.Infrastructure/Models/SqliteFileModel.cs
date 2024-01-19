using MyFileSpace.SharedKernel.DTOs;
using SQLite;
namespace MyFileSpace.Infrastructure.Models
{
    [Table("Blogs")]
    internal class SqliteFileModel : FileData
    {
        new public Guid Guid { get; set; }

        new public string OriginalName { get; set; }

        new public DateTime ModifiedOn { get; set; }

        new public long SizeInBytes { get; set; }

        new public string StoredName => throw new NotImplementedException();
    }
}
