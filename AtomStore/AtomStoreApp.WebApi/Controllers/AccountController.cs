using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AtomStore.Data.Entities;
using AtomStoreApp.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AtomStoreApp.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILoggerFactory loggerFactory, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _config = config;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
                if (!result.Succeeded)
                {
                    return new BadRequestObjectResult(result.ToString());
                }
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim("fullName", user.FullName),
                    new Claim("avatar", string.IsNullOrEmpty(user.Avatar)? string.Empty:user.Avatar),
                    new Claim("roles", string.Join(";", roles)),
                    new Claim("permissions",""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                _logger.LogError(_config["Token"]);
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds);
                _logger.LogInformation(1, "User logged in,");
                return new OkObjectResult(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return new BadRequestObjectResult("Login failure");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(model);
            }
            var user = new AppUser { FullName = "Demo", UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //user claim for write customer data
                await _userManager.AddClaimAsync(user, new Claim("Customer", "Write"));

                await _signInManager.SignInAsync(user, false);
                _logger.LogInformation(3, "User created a new account with password.");
                return new OkObjectResult(model);
            }
            return new BadRequestObjectResult(model);
        }
    }
}
