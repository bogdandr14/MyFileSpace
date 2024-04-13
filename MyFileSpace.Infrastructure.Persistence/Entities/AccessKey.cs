using MyFileSpace.Infrastructure.Persistence.Entities.Base;
using MyFileSpace.Infrastructure.Persistence.Interfaces;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class AccessKey : BaseRootEntity<int>, IRootEntity<int>
    {
        public string Key { get; set; }
        public DateTime ExpiresAt { get; set; }

        #region "Navigation properties"
        public virtual FileAccessKey? FileAccess { get; set; }
        public virtual DirectoryAccessKey? DirectoryAccess { get; set; }
        #endregion
    }
}
