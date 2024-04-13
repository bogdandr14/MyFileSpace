using MyFileSpace.Infrastructure.Persistence.Entities.Base;
using MyFileSpace.Infrastructure.Persistence.Interfaces;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class StoredFile : BaseAuditableEntity<Guid>, IRootEntity<Guid>, IAuditableEntity
    {
        public Guid OwnerId { get; set; }
        public Guid DirectorId { get; set; }

        public string Name { get; set; }
        public AccessType AccessLevel { get; set; }
        public long SizeInBytes { get; set; }
        public string ContentType { get; set; }

        public bool IsDeleted { get; set; }

        #region "Navigation properties"
        public virtual User Owner { get; set; }
        public virtual VirtualDirectory Directory { get; set; }
        public virtual ICollection<UserFileAccess> AllowedUsers { get; set; }
        public virtual FileAccessKey? FileAccessKey { get; set; }
        #endregion
    }
}
