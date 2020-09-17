using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                Id = "pengqian1",
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
        
    }
}