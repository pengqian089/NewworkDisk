using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Pengqian.NetworkDisk.Web.Controllers
{
    [ApiController,Authorize]
    public class DiskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 网盘服务器地址
        /// </summary>
        private readonly string _networkDiskRootPath;
        public DiskController(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
            _networkDiskRootPath = _configuration.GetSection("NetworkDiskRootPath").Get<string>();
        }
        
        [HttpGet,Route("api/[controller]/myFolder")]
        public FolderDirectory GetFolders(string path = "")
        {
            
            var absolutePath = Path.Combine(_networkDiskRootPath, path);
            var dirInfo = new DirectoryInfo(absolutePath);
            if (!dirInfo.Exists) return null;

            return DirectoryResult(dirInfo);
        }

        private FolderDirectory DirectoryResult(DirectoryInfo dirInfo)
        {
            var files = dirInfo.GetFileSystemInfos();
            
            var dirDetails = from x in files
                orderby x.Name
                where x.Attributes.HasFlag(FileAttributes.Directory)
                let allDir = x.FullName.Split(Path.DirectorySeparatorChar).ToList()
                let url = from y in allDir
                    where allDir.IndexOf(y) > 1
                    select y
                select new DirectoryDetail
                {
                    Name = x.Name,
                    CurrentPath = url.ToArray(),
                };
            var fileDetails = from x in files
                orderby x.Name
                where !x.Attributes.HasFlag(FileAttributes.Directory)
                let allDir = x.FullName.Split(Path.DirectorySeparatorChar).ToList()
                let url = from y in allDir
                    where allDir.IndexOf(y) > 1
                    select y
                
                select new DirectoryDetail
                {
                    Name = x.Name,
                    CurrentPath = url.ToArray()
                };
            return new FolderDirectory
            {
                IsRoot = true,
                Directories = dirDetails.ToArray(),
                Files = fileDetails.ToArray(),
                //PrevUrl = ,
                //CurrentPath = currentPath,
            };
        }
       
    }

    public class FolderDirectory
    {
        public bool IsRoot { get; set; } = false;
        
        public DirectoryDetail[] Directories { get; set; }
        
        public DirectoryDetail[] Files { get; set; }
        
        public string PrevUrl { get; set; }
        
        public string CurrentPath { get; set; }

    }
    
    public class DirectoryDetail
    {
        public string Name { get; set; }
        public string[] CurrentPath { get; set; }
    }
}