
namespace MyFileSpace.SharedKernel.DTO
{
    public class FileDTO
    {
        public int Id { get; set; }
        public string MimeType { get; set; }
        public Stream Stream { get; set; }
        public long SizeInBytes { get; set; }
        public Guid Name { get; set; }
        public string FileName => $"{Name}.{Extension}";
        public string Extension { get; }
        public byte[] FileInBytes { get; set; }
    }
}
