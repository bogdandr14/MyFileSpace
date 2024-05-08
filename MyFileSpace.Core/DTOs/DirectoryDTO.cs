using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class DirectoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentDirectoryId { get; set; }
        public AccessType AccessLevel { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
