﻿namespace MyFileSpace.Core.DTOs
{
    public class FileDetailsDTO : FileDTO
    {
        public Guid DirectorId { get; set; }
        public string Path { get; set; }
    }
}
