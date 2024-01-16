
using Microsoft.AspNetCore.Http;
using MyFileStorage.Api.DTO;

namespace MyFileStorage.Api.Extensions
{
    public static class ExtensionMethods
    {
        public static FileDTO ToImageFile(this IFormFile file)
        {
            if (file is null)
            {
                return null;
            }

            return new FileDTO
            {
                Name = Guid.NewGuid(),
                MimeType = file.ContentType,
                Stream = file.OpenReadStream(),
                SizeInBytes = file.Length
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
