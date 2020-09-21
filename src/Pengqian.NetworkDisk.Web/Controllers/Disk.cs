using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Pengqian.NetworkDisk.Web.Controllers
{
    [ApiController,Authorize]
    public class Disk : ControllerBase
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 网盘服务器地址
        /// </summary>
        private readonly string _networkDiskRootPath;
        public Disk(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
            _networkDiskRootPath = _configuration.GetSection("NetworkDiskRootPath").Get<string>();
        }
        
        [HttpGet,Route("api/[controller]/myFolder")]
        public void GetFolders()
        {
            
        }
       
    }
}