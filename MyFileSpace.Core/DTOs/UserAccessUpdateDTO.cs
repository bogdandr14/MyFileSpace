namespace MyFileSpace.Core.DTOs
{
    public class UserAccessUpdateDTO : AccessDTO
    {
        public List<Guid> AddUserIds { get; set; }
        public List<Guid> RemoveUserIds { get; set; }
    }
}
