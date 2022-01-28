using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Services;
using Reports.Infra.Tools;
using Reports.WebApp.Models.Auth;

namespace Reports.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IPersonService _personService;

        public AuthController(IAuthService authService, IPersonService personService)
        {
            _authService = authService;
            _personService = personService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["people"] = ControllerUtils.SelectedListOfDirectors(null, _personService.GetPersonsList());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Fill all fields";
                return View();
            }

            try
            {
                Guid id = _authService.Login(model.Username, model.Password);
                Auth(id);
                return RedirectToAction("Index", "Home");
            }
            catch (AuthException e)
            {
                ViewData["error"] = e.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Fill all fields";
                return Register();
            }
            if (!model.Password.Equals(model.PasswordConfirm))
            {
                ViewData["error"] = "Passwords should be the same";
                return Register();
            }

            try
            {
                Guid id;
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if ("".Equals(model.DirectorId))
                {
                    id = _authService.Register(model.Username, model.Password, model.Name, model.Surname);
                }
                else
                {
                    id = _authService.Register(model.Username, model.Password, model.Name, model.Surname, Guid.Parse(model.DirectorId));
                }

                Auth(id);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ViewData["error"] = e.Message;
                return Register();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private void Auth(Guid id)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, id.ToString()),
            };

            var identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}