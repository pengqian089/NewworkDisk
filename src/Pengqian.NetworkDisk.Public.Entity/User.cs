using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pengqian.NetworkDisk.Infrastructure;
using Pengqian.NetworkDisk.Infrastructure.Enum;

namespace Pengqian.NetworkDisk.Public.Entity
{
    public class User:IBaseEntity
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public string Password { get; set; }
        
        [BsonRepresentation(BsonType.String)] 
        public Permissions? Permissions { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }
        
        public UserInfo GetUserInfo()
        {
            return new UserInfo
            {
                Id = this.Id,
                Name = this.Name,
                Permissions = Permissions
            };
        }
    }

    public class UserInfo
    {
        public UserInfo()
        {
            LastAccessTime = DateTime.Now;
        }

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