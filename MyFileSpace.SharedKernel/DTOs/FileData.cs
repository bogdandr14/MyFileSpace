namespace MyFileSpace.SharedKernel.DTOs
{
    public class FileData
    {
        public Guid Guid { get; set; }

        public string OriginalName { get; set; }

        public string ContentType { get; set; }

        public DateTime ModifiedOn { get; set; }

        public long SizeInBytes { get; set; }

    }
}
