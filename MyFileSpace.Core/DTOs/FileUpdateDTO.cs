using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class FileUpdateDTO
    {
        public Guid FileId { get; set; }
        public string? Name { get; set; }
        public int AccessLevel { get; set; }
    }
}
