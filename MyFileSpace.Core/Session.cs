using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core
{
    public class Session
    {
        public bool IsAuthenticated{ get; set; }
        public Guid UserId { get; set; }
        public RoleType Role { get; set; }
    }
}
