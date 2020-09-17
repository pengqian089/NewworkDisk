using System;
using Pengqian.NetworkDisk.Infrastructure;
using Pengqian.NetworkDisk.Infrastructure.Enum;
using Pengqian.NetworkDisk.Public.Entity;

namespace Pengqian.NetworkDisk.Public.ViewModel
{
    public class VmUser:IMapFrom<User>
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }
        public Permissions? Permissions { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }
        
        public VmUserInfo GetUserInfo()
        {
            return new VmUserInfo
            {
                Id = this.Id,
                Name = this.Name,
                Permissions = Permissions
            };
        }
    }
}