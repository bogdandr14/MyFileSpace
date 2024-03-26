using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class DirectoryUpdateDTO
    {
        public string? Path { get; set; }
        public AccessType? AccessLevel { get; set; }
    }
}
