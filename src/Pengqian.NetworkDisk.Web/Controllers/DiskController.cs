using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pengqian.NetworkDisk.Public.ViewModel;
using Pengqian.NetworkDisk.Service;
using Pengqian.NetworkDisk.Web.Library;
using Pengqian.NetworkDisk.Web.Models;

namespace Pengqian.NetworkDisk.Web.Controllers
{
    [ApiController, Authorize]
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

        [HttpGet, Route("api/[controller]/myFolder")]
        public FolderDirectory GetFolders(string path = "")
        {
            var userInfo = User.GetUserInfo();
            var virtualPath = path?.Split("/") ?? new string[0];
            var absolutePath = Path.Combine(Path.Combine(_networkDiskRootPath, userInfo.Id), Path.Combine(virtualPath));
            //var absolutePath = Path.Combine(myPath, path);
            var dirInfo = new DirectoryInfo(absolutePath);
            if (!dirInfo.Exists) return null;

            return DirectoryResult(dirInfo, virtualPath);
        }

        private FolderDirectory DirectoryResult(DirectoryInfo dirInfo, string[] virtualPath)
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
            var listVp = virtualPath.ToList();
            if (listVp.Count > 0)
            {
                listVp.RemoveAt(virtualPath.Length - 1);
            }

            return new FolderDirectory
            {
                IsRoot = virtualPath.Length == 0,
                Directories = dirDetails.ToArray(),
                Files = fileDetails.ToArray(),
                PrevUrl = listVp.ToArray(),
                CurrentPath = virtualPath,
            };
        }

        [HttpPost, Route("api/[controller]/create")]
        public IActionResult CreateDir([FromBody] FolderModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Invalid Request");
            }

            var userInfo = User.GetUserInfo();
            var virtualPath = model.Path?.Split("/") ?? new string[0];
            var absolutePath = Path.Combine(Path.Combine(_networkDiskRootPath, userInfo.Id), Path.Combine(virtualPath),
                model.Name);
            if (!Directory.Exists(absolutePath))
                Directory.CreateDirectory(absolutePath);
            return Ok();
        }

        [HttpGet, Route("api/[controller]/search")]
        public async Task<List<VmNetworkDiskFile>> Search(string key = "")
        {
            var list = new List<VmNetworkDiskFile>();
            if (string.IsNullOrEmpty(key)) return list;
            var userInfo = User.GetUserInfo();
            var service = new NetworkDiskFileService(_networkDiskRootPath);
            list = await service.SearchMyFile(key, userInfo);
            return list;
        }

        [HttpPost, Route("api/[controller]/file")]
        public async Task<IActionResult> DeleteFile([FromBody] FolderModel model)
        {
            if (string.IsNullOrEmpty(model.Name)) return BadRequest("Invalid Request");
            var userInfo = User.GetUserInfo();
            var virtualPath = string.IsNullOrEmpty(model.Path) ? new string[0] : model.Path?.Split("/");
            var service = new NetworkDiskFileService(_networkDiskRootPath);
            await service.Delete(virtualPath, model.Name, userInfo);
            return Ok();
        }

        [HttpPost, Route("api/[controller]/dir")]
        public async Task<IActionResult> DeleteDir([FromBody] FolderModel model)
        {
            var userInfo = User.GetUserInfo();
            var virtualPath = model.Path?.Split("/") ?? new string[0];
            var service = new NetworkDiskFileService(_networkDiskRootPath);
            await service.Delete(virtualPath, userInfo);
            return Ok();
        }

        [HttpPost, Route("api/[controller]/renameFile")]
        public async Task<IActionResult> RenameFile([FromBody] RenameFileModel model)
        {
            if (string.IsNullOrEmpty(model.FileName) || string.IsNullOrEmpty(model.NewFileName))
            {
                return BadRequest("Invalid Request");
            }

            var userInfo = User.GetUserInfo();
            var virtualPath = string.IsNullOrEmpty(model.Path) ? new string[0] : model.Path?.Split("/");
            var service = new NetworkDiskFileService(_networkDiskRootPath);
            await service.Rename(virtualPath, model.FileName, userInfo, model.NewFileName);
            return Ok();
        }

        [HttpPost, Route("api/[controller]/renameFolder")]
        public async Task<IActionResult> RenameFolder([FromBody] RenameFolderModel model)
        {
            if (string.IsNullOrEmpty(model.NewName))
            {
                return BadRequest("Invalid Request");
            }

            var userInfo = User.GetUserInfo();
            var virtualPath = model.Path?.Split("/") ?? new string[0];
            var service = new NetworkDiskFileService(_networkDiskRootPath);
            await service.RenameFolder(virtualPath, userInfo, model.NewName);
            return Ok();
        }
    }

    public class FolderDirectory
    {
        public bool IsRoot { get; set; } = false;

        public DirectoryDetail[] Directories { get; set; }

        public DirectoryDetail[] Files { get; set; }

        public string[] PrevUrl { get; set; }

        public string[] CurrentPath { get; set; }
    }

    public class DirectoryDetail
    {
        public string Name { get; set; }
        public string[] CurrentPath { get; set; }
    }
}