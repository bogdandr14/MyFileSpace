using MyFileSpace.Infrastructure.Entities.Base;
using MyFileSpace.Infrastructure.Interfaces;

namespace MyFileSpace.Infrastructure.Entities
{
    public class AccessKey : BaseRootEntity<int>, IRootEntity<int>
    {
        public string Key { get; set; }
        public DateTime ExpiresAt { get; set; }

        #region "Navigation properties"
        public virtual FileAccessKey? FileAccess { get; set; }
        public virtual DirectoryAccessKey? DirectoryAccess { get; set; }
        public virtual UserAccessKey? UserAccess { get; set; }
        #endregion
    }
}
