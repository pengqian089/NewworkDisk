using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pengqian.NetworkDisk.Infrastructure.Enum;
using Pengqian.NetworkDisk.Public.ViewModel;
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
        public async Task<IActionResult> RequestToken([FromBody]AccountModel model)
        {
            var info = await _authService.IsAuthenticated(model.Account, model.Password);
            if (info != null)
            {
                return Ok(info);
            }
            return BadRequest("Invalid Request");
        }

       
    }
}