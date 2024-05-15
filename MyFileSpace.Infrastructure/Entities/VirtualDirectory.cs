using MyFileSpace.Infrastructure.Entities.Base;
using MyFileSpace.Infrastructure.Interfaces;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure.Entities
{
    public class VirtualDirectory : BaseAuditableEntity<Guid>, IRootEntity<Guid>
    {
        public Guid? ParentDirectoryId { get; set; }
        public Guid OwnerId { get; set; }
        public string VirtualPath { get; set; }
        public AccessType AccessLevel { get; set; }
        public bool IsDeleted { get; set; }

        #region "Navigation properties"
        public virtual User Owner { get; set; }
        public virtual VirtualDirectory? ParentDirectory { get; set; }
        public virtual ICollection<VirtualDirectory> ChildDirectories { get; set; }
        public virtual ICollection<StoredFile> FilesInDirectory { get; set; }
        public virtual ICollection<UserDirectoryAccess> AllowedUsers { get; set; }
        public virtual DirectoryAccessKey? DirectoryAccessKey { get; set; }
        #endregion
    }
}
