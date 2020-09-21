using System.Threading.Tasks;
using Pengqian.NetworkDisk.Public.ViewModel;
using Pengqian.NetworkDisk.Service;

namespace Pengqian.NetworkDisk.Web.Library
{
    public interface IAccountService
    {
        Task<VmUserInfo> IsValid(string account,string pwd);
    }

    public class AccountService:IAccountService
    {
        public async Task<VmUserInfo> IsValid(string account, string pwd)
        {
            var service = new UserService();
            var userInfo = await service.Login(account, pwd);
            return userInfo;
        }
    }
}