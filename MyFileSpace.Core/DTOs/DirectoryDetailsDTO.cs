﻿namespace MyFileSpace.Core.DTOs
{
    public class DirectoryDetailsDTO: DirectoryDTO
    {
        public string OwnerTagName { get; set; }
        public List<FileDTO> Files { get; set; }
        public List<DirectoryDTO> ChildDirectories { get; set; }
        public List<DirectoryDTO> PathParentDirectories { get; set; }
        public List<string> AllowedUsers { get; set; }
        public KeyAccessDetailsDTO? AccessKey {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
