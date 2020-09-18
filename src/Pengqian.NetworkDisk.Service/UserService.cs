using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pengqian.NetworkDisk.Infrastructure;
using Pengqian.NetworkDisk.Public.Entity;
using Pengqian.NetworkDisk.Public.ViewModel;

namespace Pengqian.NetworkDisk.Service
{
    public class UserService : BasicService<User>
    {
        public UserService() : base()
        {
        }

        public UserService(DbOption option) : base(option)
        {
        }

        public async Task Register(VmUser viewModel)
        {
            var user = Mapper.Map<User>(viewModel);
            var dbUser = await Repository.SearchFor(x => x.Id == user.Id).SingleOrDefaultAsync();
            if (dbUser != null) throw new Exception($"帐号‘{user.Id}’已被使用！");
            user.Password = (user.Id + user.Password).GenerateMd5();
            await Repository.InsertAsync(user);
        }
        
        public async Task<VmUserInfo> GetUserInfo(string account)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, account);
            var user = await Repository.Collection.Find(filter).SingleOrDefaultAsync();
            return Mapper.Map<VmUserInfo>(user.GetUserInfo());
        }
        
        public async Task<VmUserInfo> Login(string account, string pwd)
        {
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.Id, account),
                Builders<User>.Filter.Eq(x => x.Password, (account + pwd).GenerateMd5()));
            var user = await Repository.Collection.Find(filter).SingleOrDefaultAsync();
            return Mapper.Map<VmUserInfo>(user?.GetUserInfo());
        }
    }
}