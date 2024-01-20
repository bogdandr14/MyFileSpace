
using Microsoft.AspNetCore.Http;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Api.Extensions
{
    public static class ExtensionMethods
    {
        public static FileDTO CreateNewFileDTO(this IFormFile file)
        {
            if (file is null)
            {
                throw new ArgumentNullException("The file is null");
            }

            return new FileDTO
            {
                Guid = Guid.NewGuid(),
                ContentType = file.ContentType,
                OriginalName = file.FileName,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                SizeInBytes = file.Length,
            };
        }

        public static FileDTO UpdateExistingFileDTO(this IFormFile file, FileDTO fileDto)
        {
            if (!fileDto.OriginalName.Split('.').Last().Equals(file.FileName.Split('.').Last()))
            {
                throw new Exception("Incorrect file format");
            }

            if (fileDto.ContentType != file.ContentType)
            {
                throw new Exception($"Can not change content type of the file from ${fileDto.ContentType} to ${file.ContentType}");
            }

            fileDto.OriginalName = file.FileName;
            fileDto.ModifiedAt = DateTime.Now;
            fileDto.SizeInBytes = file.Length;

            return fileDto;
        }

        public static string StoredFileName(this FileDTO file)
        {
            //return $"{file.Guid}.{file.OriginalName.Split(".").Last()}";
            return $"{file.Guid}";
        }

    }
}
