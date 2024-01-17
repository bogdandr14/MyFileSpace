using CsvHelper.Configuration.Attributes;

namespace MyFileSpace.SharedKernel.DTOs
{
    public class FileData
    {
        [Name("FileGuid")]
        public Guid Guid { get; set; }

        [Name("FileName")]
        public string Name { get; set; }
        
        [Name("FilePath")]
        public string? Path { get; set; }

        [Name("ModifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [Name("SizeInBytes")]
        public long SizeInBytes { get; set; }
    }
}
