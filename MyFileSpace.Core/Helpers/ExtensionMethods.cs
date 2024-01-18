
using Microsoft.AspNetCore.Http;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Api.Extensions
{
    public static class ExtensionMethods
    {
        public static FileData ToFileData(this IFormFile file)
        {
            if (file is null)
            {
                throw new ArgumentNullException("The file is null");
            }

            Guid newObjectGuid = Guid.NewGuid();
            return new FileData
            {
                Guid = newObjectGuid,
                SizeInBytes = file.Length,
                OriginalName = file.FileName,
                ModifiedOn = DateTime.Now,
            };
        }
    }
}
