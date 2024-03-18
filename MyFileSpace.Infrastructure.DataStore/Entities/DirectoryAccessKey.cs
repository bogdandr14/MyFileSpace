using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class DirectoryAccessKey : IGenericEntity
    {
        public Guid DirectoryId { get; set; }
        public int AccessKeyId { get; set; }

        #region "Navigation properties"
        public virtual AccessKey AccessKey { get; set; }
        public virtual VirtualDirectory AccessibleDirectory { get; set; }
        #endregion
    }
}
