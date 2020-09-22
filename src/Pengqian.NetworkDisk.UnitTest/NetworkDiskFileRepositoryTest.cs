using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pengqian.NetworkDisk.Infrastructure;
using Pengqian.NetworkDisk.Public.ViewModel;
using Pengqian.NetworkDisk.Service;
using RestSharp;

namespace Pengqian.NetworkDisk.UnitTest
{
    [TestClass]
    public class NetworkDiskFileRepositoryTest
    {
        private readonly NetworkDiskFileService _service = new NetworkDiskFileService(@"H:\NetworkDisk Root");

        private readonly IRestClient _client = new RestClient()
        {
            UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36",
            ThrowOnAnyError = true,
            RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
        };

        [TestMethod]
        public async Task UploadTest()
        {
            var request = new RestRequest("https://core.dpangzi.com/Home/Image/5f6422ff7f661701789a29ed");
            var response = await _client.ExecuteGetAsync(request);
            var buffer = response.RawBytes;

            Assert.IsNotNull(buffer);

            var userService = new UserService();
            var userInfo = await userService.GetUserInfo("pengqian");

            Assert.IsNotNull(userInfo);

            using var md5 = MD5.Create();
            var md5Value = BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "").ToLower();
            await using var stream = new MemoryStream(buffer);

            var viewModel = new VmNetworkDiskFile
            {
                FileName = "test-5.jpg",
                FileSize = stream.Length,
                Md5 = md5Value,
                Owner = userInfo,
                //Path = new[] {"第一级目录", "第二级目录", "第三级目录","第4.1级目录"},
                Path = new[] {"第一级目录"},
                UploadTime = DateTime.Now
            };
            await _service.Upload(viewModel, stream);

            var list = await _service.SearchMyFile(viewModel.FileName, userInfo);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public async Task SearchFileTest()
        {
            var userService = new UserService();
            var userInfo = await userService.GetUserInfo("pengqian");
            Assert.IsNotNull(userInfo);

            var list = await _service.SearchMyFile("apple", userInfo);
            Assert.IsTrue(list.Count > 0);
            list.ForEach(x => { Console.WriteLine($"fileName:{x.FileName}------Path:{string.Join("-", x.Path)}"); });
        }

        [TestMethod]
        public void StartWithTest()
        {
            var array = new[] {"a", "b", "c", "d", "e"};
            var array2 = new[] {1, 2, 3, 4, 5, 6, 7};

            var result = array.StartWith(new[] {"a", "b", "c"});
            Assert.IsTrue(result);
            var result2 = array.StartWith(new[] {"a", "b", "c", "d", "e"});
            Assert.IsTrue(result2);
            var result3 = array.StartWith(new[] {"b"});
            Assert.IsFalse(result3);

            var intResult = array2.StartWith(new[] {1, 2, 3});
            Assert.IsTrue(intResult);
            var intResult2 = array2.StartWith(new[] {1, 2, 3, 4, 5, 6, 7});
            Assert.IsTrue(intResult2);
            var intResult3 = array2.StartWith(new[] {0});
            Assert.IsFalse(intResult3);
            var intResult4 = array2.StartWith(new[] {1, 2, 3, 4, 5, 6, 7, 8});
            Assert.IsFalse(intResult4);
        }

        private async Task UploadFile(string fileName, string[] path, VmUserInfo userInfo)
        {
            var request = new RestRequest("https://core.dpangzi.com/Home/Image/5f6422ff7f661701789a29ed");
            var response = await _client.ExecuteGetAsync(request);
            var buffer = response.RawBytes;
            using var md5 = MD5.Create();
            var md5Value = BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "").ToLower();
            await using var stream = new MemoryStream(buffer);

            var viewModel = new VmNetworkDiskFile
            {
                FileName = fileName,
                FileSize = stream.Length,
                Md5 = md5Value,
                Owner = userInfo,
                Path = path,
                UploadTime = DateTime.Now
            };
            await _service.Upload(viewModel, stream);
        }

        [TestMethod]
        public async Task DeleteFileTest()
        {
            var userService = new UserService();
            var userInfo = await userService.GetUserInfo("pengqian");
            Assert.IsNotNull(userInfo);

            const string fileName = "test-999.jpg";
            var path = new[] {"第一级目录"};
            await UploadFile(fileName, path, userInfo);
            var list = await _service.SearchMyFile(fileName, userInfo);

            Assert.IsTrue(list.Count > 0);
            Console.WriteLine(list.Count);

            await _service.Delete(path, fileName, userInfo);

            var list2 = await _service.SearchMyFile(fileName, userInfo);
            Assert.IsTrue(list2.Count < list.Count);
            Console.WriteLine(list2.Count);
        }

        [TestMethod]
        public async Task RenameTest()
        {
            var userService = new UserService();
            var userInfo = await userService.GetUserInfo("pengqian");
            Assert.IsNotNull(userInfo);

            const string fileName = "test-999.jpg";
            var path = new[] {"第一级目录"};
            await UploadFile(fileName, path, userInfo);
            var list = await _service.SearchMyFile(fileName, userInfo);

            Assert.IsTrue(list.Count > 0);
            Console.WriteLine(list.Count);
            
            await _service.Rename(path, fileName, userInfo,"test-997.jpg");
            var list2 = await _service.SearchMyFile(fileName, userInfo);
            Assert.IsTrue(list2.Count < list.Count);
            Console.WriteLine(list2.Count);
        }

        [TestMethod]
        public async Task RenameFolderTest()
        {
            var userService = new UserService();
            var userInfo = await userService.GetUserInfo("pengqian");
            Assert.IsNotNull(userInfo);
            
            var path = new[] {"第一层"};
            await _service.RenameFolder(path, userInfo,"the first level");
        }

        private async Task<string[]> UploadDirFiles(string name, string rootPath, VmUserInfo userInfo)
        {
            var request = new RestRequest("https://core.dpangzi.com/Home/Image/5ee2f09558eec0118018b004");
            var response = await _client.ExecuteGetAsync(request);
            var buffer = response.RawBytes;
            using var md5 = MD5.Create();
            var md5Value = BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "").ToLower();
            await using var stream = new MemoryStream(buffer);


            var dir = Enumerable.Range(0, 8).Select(x => rootPath + (x == 0 ? "" : x.ToString())).ToArray();
            var random = new Random();

            for (var i = 0; i < 50; i++)
            {
                stream.Position = 0;
                var index = random.Next(1, dir.Length + 1);
                var path = new List<string>();
                var fileName = $"{name}{i}.jpg";
                for (var j = 0; j < index; j++)
                {
                    path.Add(dir[j]);
                }

                var viewModel = new VmNetworkDiskFile
                {
                    FileName = fileName,
                    FileSize = stream.Length,
                    Md5 = md5Value,
                    Owner = userInfo,
                    Path = path.ToArray(),
                    UploadTime = DateTime.Now
                };
                await _service.Upload(viewModel, stream);
            }

            return dir;
        }

        [TestMethod]
        public async Task DeleteDirTest()
        {
            var userService = new UserService();
            var userInfo = await userService.GetUserInfo("pengqian");
            Assert.IsNotNull(userInfo);

            const string name = "dir-test-file-";
            const string rootPath = "test-dir";

            //模拟批量上传文件 并断言上传是否成功
            var dir = await UploadDirFiles(name, rootPath, userInfo);
            var list = await _service.SearchMyFile(name, userInfo);
            Console.WriteLine(list.Count);
            Assert.IsTrue(list.Count > 0);


            //删除目录，并且删除目录下的文件和DB记录
            await _service.Delete(dir, userInfo);
            var list2 = await _service.SearchMyFile(name, userInfo);
            Console.WriteLine("delete last dir:" + list2.Count);
            await _service.Delete(new[] {rootPath}, userInfo);
            var list3 = await _service.SearchMyFile(name, userInfo);

            Console.WriteLine(list3.Count);
            Assert.IsTrue(list3.Count < list.Count);
        }
    }
}