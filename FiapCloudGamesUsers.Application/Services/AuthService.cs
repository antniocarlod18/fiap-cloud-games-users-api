using FiapCloudGamesUsers.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using FiapCloudGamesUsers.Application.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace FiapCloudGamesUsers.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;

        public AuthService(IUserService userService, IConfiguration configuration)
        {
            this._userService = userService;
            _issuer = configuration["Authentication:Issuer"];
            _audience = configuration["Authentication:Audience"];
            _secretKey = configuration["Authentication:Key"];
        }

        public async Task<AuthResponseDto> GenerateJwtTokenAsync(AuthRequestDto authRequestDto)
        {
            var user = await _userService.AuthenticateAsync(new UserAuthenticateRequestDto
            {
                Email = authRequestDto.Email,
                Password = authRequestDto.Password
            });

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Active ? (user.IsAdmin ? "Admin" : "User") : "LockUser")
            };

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(30);

            var tokenOptions = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires,
                signingCredentials: signinCredentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new AuthResponseDto { Token=tokenString, Expiration= expires };
        }

        public Guid GetUserIDAsync(HttpContext context)
        {
            var claims = context.User.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim?.Value, out var userId);
            return userId;
        }
    }
}
