using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool StartWith<T>(this IEnumerable<T> source, IEnumerable<T> value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            
            if (Equals(source, value)) return true;
            var arrayValues = value as T[] ?? value.ToArray();
            var arraySource = source as T[] ?? source.ToArray();
            if (!arrayValues.Any()) return true;

            if (arrayValues.Length > arraySource.Length) return false;
            var index = 0;
            return arrayValues.All(x => x.Equals(arraySource[index++]));
        }
    }
}