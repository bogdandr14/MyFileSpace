using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSpace.Core.DTOs
{
    public class FileDownloadDTO
    {
        public byte[] Content { get; set; }
        public string DownloadName { get; set; }
        public DateTime LastModified { get; set; }
        public string ContentType {  get; set; }
    }
}
