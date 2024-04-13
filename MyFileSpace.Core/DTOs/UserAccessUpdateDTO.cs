namespace MyFileSpace.Core.DTOs
{
    public class UserAccessUpdateDTO : AccessDTO
    {
        public List<Guid> UserGuids { get; set; }
    }
}
