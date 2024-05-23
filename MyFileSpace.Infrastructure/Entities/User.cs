﻿using MyFileSpace.Infrastructure.Entities.Base;
using MyFileSpace.Infrastructure.Interfaces;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure.Entities
{
    public class User: BaseRootEntity<Guid>, IRootEntity<Guid>
    {
        public string TagName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsConfirmed {  get; set; }
        public RoleType Role { get; set; }
        public DateTime LastPasswordChange { get; set; }
        public string Salt { get; set; }

        #region "Navigation properties"
        public virtual ICollection<VirtualDirectory> Directories { get; set; }
        public virtual ICollection<StoredFile> Files { get; set; }
        public virtual ICollection<UserDirectoryAccess> AllowedDirectories { get; set; }
        public virtual ICollection<UserFileAccess> AllowedFiles { get; set; }
        public virtual ICollection<FavoriteFile> FavoriteFiles { get; set; }
        public virtual ICollection<UserAccessKey> UserAccessKeys { get; set; }
        #endregion
    }
}