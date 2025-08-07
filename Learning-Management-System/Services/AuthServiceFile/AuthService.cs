using Learning_Management_System.DTOs.Auth;
using Learning_Management_System.Models;
using Learning_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Learning_Management_System.Services.AuthServiceFile
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // ✅ User registration with role assignment
        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                // Assign role (must be pre-created via seeding or RoleManager)
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            return result;
        }

        // ✅ Login logic with role and token generation
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid email or password." };

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return new AuthResponseDto { Success = false, Message = "Invalid email or password." };

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Student";

            var (accessToken, expiresAt) = GenerateJwtToken(user, role);
            var refreshToken = Guid.NewGuid().ToString(); // Placeholder

            return new AuthResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Role = role,
                Message = "Login successful"
            };
        }

        // ✅ (Demo) Refresh Token logic
        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // Dummy refresh - replace with actual refresh token logic
            var dummyUser = await _userManager.FindByEmailAsync("example@example.com");
            if (dummyUser == null) return null;

            var roles = await _userManager.GetRolesAsync(dummyUser);
            var role = roles.FirstOrDefault() ?? "Student";

            var (accessToken, expiresAt) = GenerateJwtToken(dummyUser, role);

            return new AuthResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = dto.RefreshToken,
                ExpiresAt = expiresAt,
                FullName = dummyUser.FullName,
                Email = dummyUser.Email ?? "",
                Role = role,
                Message = "Token refreshed"
            };
        }

        // ✅ Generates JWT access token with claims
        private (string token, DateTime expiresAt) GenerateJwtToken(ApplicationUser user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FullName ?? ""),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }
}
