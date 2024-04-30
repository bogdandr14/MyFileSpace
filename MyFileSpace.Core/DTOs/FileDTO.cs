using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class FileDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public AccessType AccessLevel { get; set; }
        public int SizeInBytes { get; set; }
        public string ContentType { get; set; }
        public Guid OwnerId { get; set; }
    }
}
