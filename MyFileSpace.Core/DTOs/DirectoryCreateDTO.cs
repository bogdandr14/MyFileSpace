using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class DirectoryCreateDTO
    {
        public Guid? ParentDirectoryId { get; set; }
        public string Name { get; set; }
        public int AccessLevel { get; set; }
    }
}
