using CsvHelper.Configuration.Attributes;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Infrastructure.Models
{
    public class CsvFileData : FileData
    {
        [Name("FileGuid")]
        new public Guid Guid { get; set; }

        [Name("FileName")]
        new public string OriginalName { get; set; }

        [Name("ContentType")]
        new public string ContentType { get; set; }

        [Name("ModifiedOn")]
        new public DateTime ModifiedOn { get; set; }

        [Name("SizeInBytes")]
        new public long SizeInBytes { get; set; }

        public static CsvFileData Adapt(FileData file)
        {
            return new CsvFileData()
            {
                Guid = file.Guid,
                OriginalName = file.OriginalName,
                ContentType = file.ContentType,
                ModifiedOn = file.ModifiedOn,
                SizeInBytes = file.SizeInBytes,
            };
        }

        public FileData ToBase()
        {
            return new FileData()
            {
                Guid = Guid,
                OriginalName = OriginalName,
                ContentType = ContentType,
                ModifiedOn = ModifiedOn,
                SizeInBytes = SizeInBytes,
            };
        }
    }
}
