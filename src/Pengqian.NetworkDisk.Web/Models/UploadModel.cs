using Microsoft.AspNetCore.Http;

namespace Pengqian.NetworkDisk.Web.Models
{
    public class UploadModel
    {
        public IFormFile File { get; set; }
        
        public string Path { get; set; }
    }
}