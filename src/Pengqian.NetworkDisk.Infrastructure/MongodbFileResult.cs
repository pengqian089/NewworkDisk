using System.IO;

namespace Pengqian.NetworkDisk.Infrastructure
{
    public class MongodbFileResult
    {
        public Stream Stream { get; set; }

        public string FileName { get; set; }

        public string Md5 { get; set; }
    }
}