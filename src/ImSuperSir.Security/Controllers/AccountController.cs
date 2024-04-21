using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ImSuperSir.Security.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly SignInManager<IdentityUser> _SignInManager;
        private readonly IConfiguration _Configuration;

        public AccountController(UserManager<IdentityUser> userManager
                        , SignInManager<IdentityUser> signInManager
                        , IConfiguration configuration)
        {
            _Configuration = configuration;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserCredentials pUserCredentials)
        {
            var user = new IdentityUser
            {
                UserName = pUserCredentials.Email,
                Email = pUserCredentials.Email
            };

            var result = await _UserManager.CreateAsync(user, pUserCredentials.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(BuildToken(pUserCredentials));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials pUserCredentials)
        {
            var result = await _SignInManager.PasswordSignInAsync(pUserCredentials.Email, pUserCredentials.Password, false, false);

            if (!result.Succeeded)
            {
                return BadRequest("Invalid login attempt");
            }

            return Ok(BuildToken(pUserCredentials));
        }


        [HttpGet("Renovar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (emailClaim == null)
            {
                return BadRequest("Invalid login attempt");
            }

            var email = emailClaim.Value;

            var lUser = await _UserManager.FindByNameAsync(email);

            if (lUser == null)
            {
                return BadRequest("Invalid login attempt");
            }

            return Ok(BuildToken(new UserCredentials { Email = email }));
        }

        private AuthenticationResponse BuildToken(UserCredentials pUserCredentials)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, pUserCredentials.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var lJWTKey = _Configuration["AspNetCoreIdentityJWTKet"] ?? throw new InvalidOperationException("Missing JWT Key");
            var lKey = Encoding.UTF8.GetBytes(lJWTKey);

            var key = new SymmetricSecurityKey(lKey);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ImSuperSir.Security",
                audience: "ImSuperSir.Security",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new AuthenticationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

    }
}