using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Interfaces
{
    internal interface IAuditableEntity : IGenericEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }
    }
}
