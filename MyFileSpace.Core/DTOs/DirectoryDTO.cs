using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class DirectoryDTO
    {
        public Guid Id { get; set; }
        public string FullPath { get; set; }
        public AccessType AccessLevel { get; set; }
    }
}
