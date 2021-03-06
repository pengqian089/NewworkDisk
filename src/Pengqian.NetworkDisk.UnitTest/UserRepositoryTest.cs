using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Pengqian.NetworkDisk.Infrastructure.Enum;
using Pengqian.NetworkDisk.Public.ViewModel;
using Pengqian.NetworkDisk.Service;

namespace Pengqian.NetworkDisk.UnitTest
{
    [TestClass]
    public class UserRepositoryTest
    {
        private readonly UserService _service = new UserService();

        [TestMethod]
        public async Task RegisterTest()
        {
            var user = new VmUser
            {
                Id = "pengqian",
                Name = "彭迁",
                Password = "123456",
                Permissions = Permissions.System,
                CreateTime = DateTime.Now,
                LastUpdateTime = DateTime.Now
            };
            await _service.Register(user);

            var dbUser = await _service.GetUserInfo(user.Id);
            Assert.IsNotNull(dbUser);
        }

        [TestMethod]
        public async Task LoginTest()
        {
            var user = await _service.Login("pengqian","123456");
            Assert.IsNotNull(user);
            Console.WriteLine(JsonConvert.SerializeObject(user));
        }
    }
}