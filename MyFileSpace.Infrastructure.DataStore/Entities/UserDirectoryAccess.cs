using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class UserDirectoryAccess : IGenericEntity
    {
        public Guid DirectoryId { get; set; }
        public Guid AllowedUserId { get; set; }

        #region "Navigation properties"
        public virtual User AllowedUser { get; set; }
        public virtual VirtualDirectory Directory { get; set; }
        #endregion
    }
}
