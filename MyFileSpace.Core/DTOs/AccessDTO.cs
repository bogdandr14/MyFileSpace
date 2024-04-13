using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class AccessDTO
    {
        public Guid ObjectId { get; set; }
        public ObjectType ObjectType { get; set; }
    }
}
