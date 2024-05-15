using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Entities
{
    public class UserFileAccess : IGenericEntity
    {
        public Guid FileId { get; set; }
        public Guid AllowedUserId { get; set; }

        #region "Navigation properties"
        public virtual User AllowedUser { get; set; }
        public virtual StoredFile File { get; set; }
        #endregion
    }
}
