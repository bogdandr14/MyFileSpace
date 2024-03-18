using MyFileSpace.Infrastructure.Persistence.Interfaces;

namespace MyFileSpace.Infrastructure.Persistence.Entities.Base
{
    public class BaseRootEntity<TId> : IRootEntity<TId>
    {
        //Primary key
        public TId Id { get; set; }
    }
}
