using Microsoft.AspNetCore.Http;
using MyFileSpace.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class VirtualDirectoryService : IVirtualDirectoryService
    {
        public Task AddDirectory(IFormFile directory, Guid parentDirectoryId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDirectory(Guid directoryId)
        {
            throw new NotImplementedException();
        }

        public Task<List<FileDetailsDTO>> GetAllDirectoriesInfo()
        {
            throw new NotImplementedException();
        }

        public Task<FileDetailsDTO> GetDirectoryInfo(Guid directoryId, string? accessKey = null)
        {
            throw new NotImplementedException();
        }

        public Task MoveDirectoryToBin(Guid directoryId)
        {
            throw new NotImplementedException();
        }

        public Task MoveToDirectory(Guid directoryToMoveId, Guid newParentDirectoryId)
        {
            throw new NotImplementedException();
        }

        public Task RestoreDirectory(Guid directoryId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDirectory(IFormFile directory, Guid directoryId)
        {
            throw new NotImplementedException();
        }
    }
}
