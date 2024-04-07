using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Interfaces
{
    internal interface IAuditableEntity : IGenericEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }
    }
}
