using CsvHelper.Configuration.Attributes;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Infrastructure.Models
{
    public class CsvFileData
    {
        [Name("FileGuid")]
        public Guid Guid { get; set; }

        [Name("FileName")]
        public string OriginalName { get; set; }

        [Name("ContentType")]
        public string ContentType { get; set; }

        [Name("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Name("ModifiedAt")]
        public DateTime ModifiedAt { get; set; }

        [Name("SizeInBytes")]
        public long SizeInBytes { get; set; }

        public static CsvFileData Adapt(FileDTO file)
        {
            return new CsvFileData()
            {
                Guid = file.Guid,
                OriginalName = file.OriginalName,
                ContentType = file.ContentType,
                CreatedAt = file.CreatedAt,
                ModifiedAt = file.ModifiedAt,
                SizeInBytes = file.SizeInBytes
            };
        }

        public FileDTO ToBase()
        {
            return new FileDTO()
            {
                Guid = Guid,
                OriginalName = OriginalName,
                ContentType = ContentType,
                ModifiedAt = ModifiedAt,
                CreatedAt = CreatedAt,
                SizeInBytes = SizeInBytes
            };
        }
    }
}
