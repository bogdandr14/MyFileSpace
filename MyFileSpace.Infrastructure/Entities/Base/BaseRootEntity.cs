using MyFileSpace.Infrastructure.Interfaces;

namespace MyFileSpace.Infrastructure.Entities.Base
{
    public class BaseRootEntity<TId> : IRootEntity<TId>
    {
        //Primary key
        public TId Id { get; set; }
    }
}
