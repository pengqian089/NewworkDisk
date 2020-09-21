using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pengqian.NetworkDisk.Public.ViewModel;

namespace Pengqian.NetworkDisk.Web.Library
{
    public interface IAuthenticateService
    {
        Task<object> IsAuthenticated(string account, string pwd);
    }

    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IAccountService _accountService;
        private readonly VmTokenManagement _tokenManagement;

        public TokenAuthenticationService(IAccountService accountService, IOptions<VmTokenManagement> tokenManagement)
        {
            _accountService = accountService;
            _tokenManagement = tokenManagement.Value;
        }

        public async Task<object> IsAuthenticated(string account, string pwd)
        {
            var userInfo = await _accountService.IsValid(account, pwd);
            if (userInfo == null) return null;
            
            var claims = userInfo.GetType().GetProperties()
                .Select(x => new Claim(x.Name, x.GetValue(userInfo)?.ToString() ?? "")).ToList();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration);
            var jwtToken = new JwtSecurityToken(_tokenManagement.Issuer, _tokenManagement.Audience, claims,
                expires: expires, signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return new {expires, token, account = userInfo.Id};
        }
    }
}