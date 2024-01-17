
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
                return null;
            }

            Guid newObjectGuid = Guid.NewGuid();
            return new FileData
            {
                Guid = newObjectGuid,
                SizeInBytes = file.Length,
                Name = file.FileName,
                ModifiedOn = DateTime.Now,
                Path = $"{newObjectGuid}.{file.FileName.Split('.').Last()}"
            };
        }

        public static string GetExtension(string mimeType)
        {
            switch (mimeType.ToLower())
            {
                case "image/jpeg":
                    return "jpg";
                case "image/png":
                    return "png";
                case "image/gif":
                    return "gif";
                default:
                    throw new BadImageFormatException("File format is not supported");
            }
        }
    }
}
