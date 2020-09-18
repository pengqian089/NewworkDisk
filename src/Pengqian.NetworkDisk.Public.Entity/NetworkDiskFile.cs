using System;
using MongoDB.Bson;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.Public.Entity
{
    public class NetworkDiskFile:BaseEntity
    {
        public UserInfo Owner { get; set; }
        
        public string[] Path { get; set; }
        
        public string Md5 { get; set; }
        
        public DateTime UploadTime { get; set; }
        
        public long FileSize { get; set; }
        
        public string FileName { get; set; }
        
        
        //public string 
    }
}