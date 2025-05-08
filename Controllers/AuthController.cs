using Microsoft.AspNetCore.Authorization;
using UserEntityWebAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserEntityWebAPI.Services;
using UserEntityWebAPI.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace UserEntityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtOptions _jwtOptions;

        public AuthController(UserService userService, IOptions<JwtOptions> jwtOptions)
        {
            _userService = userService;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!_userService.TryLogin(request.Login, request.Password, out User? user))
            {
                return Unauthorized("Неверный логин или пароль");
            }

            var token = GenerateJwt(user);
            return Ok(new {Token = token});
        }

        private string GenerateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Login),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user")
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.LifetimeMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
