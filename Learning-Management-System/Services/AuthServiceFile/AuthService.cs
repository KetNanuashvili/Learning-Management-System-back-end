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
        private readonly RoleManager<IdentityRole<int>> _roleManager; // ✅ დაამატე
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager, // ✅ დაამატე
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager; // ✅
            _configuration = configuration;
        }

        // ✅ Registration
        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            // 0) დუპლიკატის შემოწმება
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return IdentityResult.Failed(new IdentityError { Description = "User with this email already exists." });

            // 1) იუზერის შექმნა
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return createResult;

            // 2) როლის განსაზღვრა (RoleId > Role > "Student")
            string roleToAssignName = "Student";

            if (dto.RoleId.HasValue)
            {
                var roleEntityById = await _roleManager.FindByIdAsync(dto.RoleId.Value.ToString());
                if (roleEntityById == null)
                {
                    // წაშალე ახლადშექმნილი იუზერი, რომ არ დაგრჩეს როლის გარეშე
                    await _userManager.DeleteAsync(user);
                    return IdentityResult.Failed(new IdentityError { Description = $"Role with id '{dto.RoleId}' not found." });
                }
                roleToAssignName = roleEntityById.Name!;
            }
            else if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                roleToAssignName = dto.Role!;
            }

            // 3) თუ ასეთი როლი არ არსებობს — შექმენი
            if (!await _roleManager.RoleExistsAsync(roleToAssignName))
            {
                var roleCreate = await _roleManager.CreateAsync(new IdentityRole<int>(roleToAssignName));
                if (!roleCreate.Succeeded)
                {
                    await _userManager.DeleteAsync(user); // rollback
                    return IdentityResult.Failed(
                        roleCreate.Errors.Any()
                            ? roleCreate.Errors.ToArray()
                            : new[] { new IdentityError { Description = $"Failed to create role '{roleToAssignName}'." } }
                    );
                }
            }

            // 4) როლის მიბმა იუზერზე
            var addRoleResult = await _userManager.AddToRoleAsync(user, roleToAssignName);
            if (!addRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user); // rollback
                return IdentityResult.Failed(
                    addRoleResult.Errors.Any()
                        ? addRoleResult.Errors.ToArray()
                        : new[] { new IdentityError { Description = $"Failed to assign role '{roleToAssignName}' to user." } }
                );
            }

            // ✅ წარმატება
            return IdentityResult.Success;
        }

        // ✅ Login
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid email or password." };

            var pwOk = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!pwOk.Succeeded)
                return new AuthResponseDto { Success = false, Message = "Invalid email or password." };

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
                return new AuthResponseDto { Success = false, Message = "User has no roles assigned." }; // ❗️აღარ ვფარავთ Student-ით

            var role = roles.First();

            var (accessToken, expiresAt) = GenerateJwtToken(user, role);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Role = role,
                Message = "Login successful"
            };
        }

        // ================== Helpers ==================

        private (string token, DateTime expiresAt) GenerateJwtToken(ApplicationUser user, string role)
        {
            var (issuer, audience, keyString, expiresMinutes) = ReadJwtOptions();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (accessToken, expiresAt);
        }

        private static string GenerateRefreshToken() => Guid.NewGuid().ToString("N");

        private (string issuer, string audience, string key, int expiresMinutes) ReadJwtOptions()
        {
            var section = _configuration.GetSection("JWT");
            if (!section.Exists())
                section = _configuration.GetSection("Jwt");

            var issuer = section["Issuer"];
            var audience = section["Audience"];
            var key = section["Key"];
            var expiresStr = section["ExpiresInMinutes"];

            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("JWT Key is missing from configuration (JWT:Key).");
            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT Issuer is missing from configuration (JWT:Issuer).");
            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT Audience is missing from configuration (JWT:Audience).");

            if (!int.TryParse(expiresStr, out var expires))
                expires = 60;

            return (issuer, audience, key, expires);
        }

        public Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
