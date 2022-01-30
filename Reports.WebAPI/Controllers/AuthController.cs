using System;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Services;

namespace Reports.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public Guid Auth(Guid id, string keyWord)
        {
            return _authService.CreateToken(id, keyWord);
        }
    }
}