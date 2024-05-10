using MyFileSpace.Infrastructure.Persistence.Entities.Base;
using MyFileSpace.Infrastructure.Persistence.Interfaces;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class User: BaseRootEntity<Guid>, IRootEntity<Guid>
    {
        public string TagName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public RoleType Role { get; set; }
        public DateTime LastPasswordChange { get; set; }
        public string Salt { get; set; }

        #region "Navigation properties"
        public virtual ICollection<VirtualDirectory> Directories { get; set; }
        public virtual ICollection<StoredFile> Files { get; set; }
        public virtual ICollection<UserDirectoryAccess> AllowedDirectories { get; set; }
        public virtual ICollection<UserFileAccess> AllowedFiles { get; set; }
        public virtual ICollection<FavoriteFile> FavoriteFiles { get; set; }
        #endregion
    }
}
