using System;
using Pengqian.NetworkDisk.Infrastructure;
using Pengqian.NetworkDisk.Public.Entity;

namespace Pengqian.NetworkDisk.Public.ViewModel
{
    public class VmNetworkDiskFile:IMapFrom<NetworkDiskFile>
    {
        public string Id { get; set; }
        
        public VmUserInfo Owner { get; set; }
        
        public string[] Path { get; set; }
        
        public string Md5 { get; set; }
        
        public DateTime UploadTime { get; set; }
        
        public long FileSize { get; set; }
        
        public string FileName { get; set; }
    }
}