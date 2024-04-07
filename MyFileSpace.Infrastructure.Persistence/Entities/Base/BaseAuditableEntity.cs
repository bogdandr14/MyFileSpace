using MyFileSpace.Infrastructure.Persistence.Interfaces;

namespace MyFileSpace.Infrastructure.Persistence.Entities.Base
{
    public class BaseAuditableEntity<TId> : BaseRootEntity<TId>, IRootEntity<TId>, IAuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
