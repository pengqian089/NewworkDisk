using System;
using System.Security.Cryptography;
using System.Text;

namespace Pengqian.NetworkDisk.Infrastructure
{
    public static class NetworkDiskExtensions
    {
        /// <summary>
        /// 生成MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GenerateMd5(this string str)
        {
            var md5Provider = new MD5CryptoServiceProvider();
            var hash = md5Provider.ComputeHash(Encoding.Default.GetBytes(str));
            var md5 = BitConverter.ToString(hash).Replace("-", "");
            return md5;
        }
    }
}