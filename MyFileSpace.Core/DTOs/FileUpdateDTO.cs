using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class FileUpdateDTO
    {
        public string? Name { get; set; }
        public AccessType? AccessLevel { get; set; }
    }
}
