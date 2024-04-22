﻿using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class DirectoryUpdateDTO
    {
        public Guid DirectoryId { get; set; }
        public string? Name { get; set; }
        public AccessType? AccessLevel { get; set; }
    }
}
