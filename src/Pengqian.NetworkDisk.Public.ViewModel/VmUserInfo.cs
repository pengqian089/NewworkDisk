using System;
using Pengqian.NetworkDisk.Infrastructure;
using Pengqian.NetworkDisk.Infrastructure.Enum;
using Pengqian.NetworkDisk.Public.Entity;

namespace Pengqian.NetworkDisk.Public.ViewModel
{
    public class VmUserInfo:IMapFrom<UserInfo>
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 权限
        /// </summary>
        public Permissions? Permissions { get; set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime? LastAccessTime { get; set; }
    }
}