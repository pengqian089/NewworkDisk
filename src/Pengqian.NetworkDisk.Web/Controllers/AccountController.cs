using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pengqian.NetworkDisk.Service;
using Pengqian.NetworkDisk.Web.Library;
using Pengqian.NetworkDisk.Web.Models;

namespace Pengqian.NetworkDisk.Web.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        public AccountController(IAuthenticateService authService)
        {
            _authService = authService;
        }
        
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> RequestToken([FromForm]AccountModel model)
        {
            var token = await _authService.IsAuthenticated(model.Account, model.Password);
            if (!string.IsNullOrEmpty(token))
            {
                return Ok(token);
            }
            return BadRequest("Invalid Request");
        }
    }
}