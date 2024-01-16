using System.ComponentModel.DataAnnotations;

namespace MyFileStorage.Api.Attributes
{
    public class FileValidationAttribute : ValidationAttribute
    {
        private readonly int _maxSizeKB;
        private int MaxFileSizeBytes
        {
            get { return _maxSizeKB * 1024; }
        }

        public FileValidationAttribute() : this(40)
        {
        }

        public FileValidationAttribute(int maxSizeKB)
        {
            _maxSizeKB = maxSizeKB;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("file is required");
            }

            var file = (value as IFormFile);

            if (file == null)
            {
                return new ValidationResult("object is not a file");
            }

            if (file.ContentType == null)
            {
                return new ValidationResult($"Invalid file type: {file.ContentType}.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return new ValidationResult($"File size exceeds {_maxSizeKB}KB.");
            }

            return ValidationResult.Success!;
        }
    }
}
