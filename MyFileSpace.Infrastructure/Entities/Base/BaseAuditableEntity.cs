using MyFileSpace.Infrastructure.Interfaces;

namespace MyFileSpace.Infrastructure.Entities.Base
{
    public class BaseAuditableEntity<TId> : BaseRootEntity<TId>, IRootEntity<TId>, IAuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
