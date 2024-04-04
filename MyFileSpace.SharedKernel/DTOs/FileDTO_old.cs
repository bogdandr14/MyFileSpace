namespace MyFileSpace.SharedKernel.DTOs
{
    public class FileDTO_old
    {
        public Guid Guid { get; set; }

        public string OriginalName { get; set; }

        public string ContentType { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime ModifiedAt { get; set; }

        public long SizeInBytes { get; set; }

    }
}
