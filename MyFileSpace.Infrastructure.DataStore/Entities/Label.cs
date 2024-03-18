using MyFileSpace.Infrastructure.Persistence.Entities.Base;
using MyFileSpace.Infrastructure.Persistence.Interfaces;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class Label: BaseRootEntity<int>, IRootEntity<int>
    {
        public Guid OwnerId { get; set; }
        public string Name { get; set; }

        #region "Navigation properties"
        public virtual User Owner { get; set; }
        public virtual ICollection<FileLabel> FilesWithLabel { get; set; }
        #endregion
    }
}
