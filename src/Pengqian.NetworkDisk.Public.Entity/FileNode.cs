using MongoDB.Bson;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.Public.Entity
{
    public class FileNode:BaseEntity
    {
        public UserInfo Owner { get; set; }
        
        public ObjectId Parent { get; set; }
        
        //public string 
    }
}