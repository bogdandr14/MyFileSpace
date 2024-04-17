using Microsoft.AspNetCore.Http;
using MyFileSpace.Core.DTOs;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core
{
    public class Session
    {
        public bool IsAuthenticated { get; set; }
        public Guid UserId { get; set; }
        public RoleType Role { get; set; }

        public string AllFilesCacheKey
        {
            get
            {
                return $"{nameof(FileDetailsDTO)}_owner_{UserId}";
            }
        }

        public string AllDirectoriesCacheKey
        {
            get
            {
                return $"{nameof(DirectoryDTO)}_owner_{UserId}";
            }
        }
    }
}
