using MyFileSpace.SharedKernel.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class UserAccessKey : IGenericEntity
    {
        public Guid UserId { get; set; }
        public UserKeyType Type { get; set; }
        public int AccessKeyId { get; set; }

        #region "Navigation properties"
        public virtual AccessKey AccessKey { get; set; }
        public virtual User User { get; set; }
        #endregion
    }
}
