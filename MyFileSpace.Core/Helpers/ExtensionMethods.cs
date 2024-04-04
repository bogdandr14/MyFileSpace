
using Microsoft.AspNetCore.Http;
using MyFileSpace.SharedKernel.DTOs;
using MyFileSpace.SharedKernel.Exceptions;

namespace MyFileSpace.Api.Extensions
{
    public static class ExtensionMethods
    {
        public static FileDTO_old CreateNewFileDTO(this IFormFile file)
        {
            if (file is null)
            {
                throw new InvalidException("The file is null");
            }

            return new FileDTO_old
            {
                Guid = Guid.NewGuid(),
                ContentType = file.ContentType,
                OriginalName = file.FileName,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                SizeInBytes = file.Length,
            };
        }

        public static FileDTO_old UpdateExistingFileDTO(this IFormFile file, FileDTO_old fileDto)
        {
            if (!fileDto.OriginalName.Split('.').Last().Equals(file.FileName.Split('.').Last()))
            {
                throw new InvalidException("Incorrect file format");
            }

            if (fileDto.ContentType != file.ContentType)
            {
                throw new InvalidException($"Can not change content type of the file from ${fileDto.ContentType} to ${file.ContentType}");
            }

            fileDto.OriginalName = file.FileName;
            fileDto.ModifiedAt = DateTime.Now;
            fileDto.SizeInBytes = file.Length;

            return fileDto;
        }

        public static string StoredFileName(this FileDTO_old file)
        {
            //return $"{file.Guid}.{file.OriginalName.Split(".").Last()}";
            return $"{file.Guid}";
        }

    }
}
