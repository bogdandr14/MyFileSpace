
using Microsoft.AspNetCore.Http;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Api.Extensions
{
    public static class ExtensionMethods
    {

        public static FileData NewFileData(this IFormFile file)
        {
            return file.ToFileData();
        }

        public static FileData ExistingFileData(this IFormFile file, Guid fileGuid)
        {
            return file.ToFileData(fileGuid);
        }

        public static string StoredFileName(this FileData file)
        {
            return $"{file.Guid}.{file.OriginalName.Split(".").Last()}";
        }

        private static FileData ToFileData(this IFormFile file, Guid? fileGuid = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException("The file is null");
            }

            return new FileData
            {
                Guid = fileGuid ?? Guid.NewGuid(),
                ContentType = file.ContentType,
                OriginalName = file.FileName,
                ModifiedOn = DateTime.Now,
                SizeInBytes = file.Length,
            };
        }
    }
}
