using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous] // Controller ini boleh diakses tanpa login
    [Tags("Authentication")]
    public class AuthController : ControllerBase
    {
        // Dependency injection for database context and configuration
        private readonly DataEntities _db;
        private readonly IConfiguration _config;

        public AuthController(DataEntities context, IConfiguration config)
        {
            _db = context;
            _config = config;
        }

        /// <summary>
        /// Login endpoint: Validates user credentials and returns JWT & refresh token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Find user by ID and status
            var user = await _db.User_Masters.FirstOrDefaultAsync(u =>
                u.user_id == loginDto.UserId &&
                u.stsrc == "A" &&
                u.user_status == "Aktif");

            if (user == null)
            {
                return Unauthorized(ApiResponse<object>.Error("User ID atau Password salah.", 401));
            }

            // Validasi password menggunakan BCrypt.Verify, sudah auto validasi salt juga
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.user_password))
            {
                return Unauthorized(ApiResponse<object>.Error("User ID atau Password salah.", 401));
            }

            // Generate tokens
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var newRefreshToken = new User_Refresh_Token
            {
                user_id = user.user_id,
                access_token = accessToken,
                refresh_token = refreshToken,
                date_expires = DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenDurationInDays")),
                ip_address = ipAddress,
                device_info = loginDto.DeviceInfo
            };
            _db.SetStsrcFields(newRefreshToken);
            _db.User_Refresh_Tokens.Add(newRefreshToken);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                accessToken,
                refreshToken,
                userId = user.user_id,
                userName = user.user_nama
            }));
        }

        /// <summary>
        /// Refresh endpoint: Issues new JWT and refresh token if the old ones are valid.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ApiResponse<object>.BadRequest("Invalid token: User identifier not found."));
            }

            // Find refresh token in database
            var savedRefreshToken = await _db.User_Refresh_Tokens.FirstOrDefaultAsync(rt =>
                rt.refresh_token == request.RefreshToken &&
                rt.access_token == request.AccessToken &&
                rt.user_id == userId &&
                rt.stsrc == "A");

            if (savedRefreshToken == null || savedRefreshToken.date_expires <= DateTime.UtcNow)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Invalid client request"));
            }

            var user = await _db.User_Masters.FindAsync(userId);
            if (user == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Invalid client request"));
            }

            // Generate new tokens
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Update refresh token in database
            savedRefreshToken.refresh_token = newRefreshToken;
            savedRefreshToken.access_token = newAccessToken;
            _db.SetStsrcFields(savedRefreshToken);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            }));
        }

        /// <summary>
        /// Logout endpoint: Deletes the refresh token from database.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            // Find refresh token in database
            var savedRefreshToken = await _db.User_Refresh_Tokens.FirstOrDefaultAsync(rt =>
                rt.refresh_token == request.RefreshToken &&
                rt.stsrc == "A");

            if (savedRefreshToken != null)
            {
                _db.DeleteStsrc(savedRefreshToken);
                await _db.SaveChangesAsync();
            }

            return Ok(ApiResponse<object>.Ok(null, "Logout berhasil."));
        }

        /// <summary>
        /// Registrasi Langkah 1: Validasi detail user dan buat token temporer.
        /// </summary>
        [HttpPost("register/check-details")]
        public async Task<IActionResult> CheckDetails([FromBody] RegisterStep1Dto step1Dto)
        {
            // Buat token temporer yang berisi data dari langkah 1
            var tempToken = GenerateTemporaryToken(step1Dto);

            return Ok(ApiResponse<object>.Ok(new { tempToken }));
        }

        /// <summary>
        /// Registrasi Langkah 2: Set password dan buat user baru.
        /// </summary>
        [HttpPost("register/complete")]
        public async Task<IActionResult> CompleteRegistration([FromBody] RegisterStep2Dto step2Dto)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = GetPrincipalFromExpiredToken(step2Dto.TempToken); // Kita pakai ulang method ini
            }
            catch (SecurityTokenException)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Sesi registrasi tidak valid atau sudah kedaluwarsa."));
            }

            var userName = principal.FindFirstValue(ClaimTypes.Name);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var address = principal.FindFirstValue(ClaimTypes.StreetAddress);
            var phone = principal.FindFirstValue(ClaimTypes.MobilePhone);

            var newUserId = await GenerateUniqueUserId();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var newUser = new User_Master
            {
                user_id = newUserId,
                user_nama = userName,
                user_email = email,
                user_alamat = address,
                user_telp = phone,
                user_password = BCrypt.Net.BCrypt.HashPassword(step2Dto.Password),
                user_status = "Aktif",
                user_main_role = "Staff",
                user_roles_csv = "Staff",
                ip_address = ipAddress,
                user_agent = step2Dto.DeviceInfo
            };

            _db.SetStsrcFields(newUser);
            _db.User_Masters.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Created(
                new { userId = newUser.user_id },
                "Registrasi berhasil."));
        }

        /// <summary>
        /// Helper: Generate JWT access token for a user.
        /// </summary>
        private string GenerateJwtToken(User_Master user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.user_id),
                new Claim(JwtRegisteredClaimNames.Name, user.user_nama),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                // Add more claims if needed
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenDurationInMinutes")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Helper: Generate a secure random refresh token.
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Helper: Extract ClaimsPrincipal from expired JWT token.
        /// </summary>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false // Accept expired tokens
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private async Task<string> GenerateUniqueUserId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string newId;
            bool isUnique;

            do
            {
                newId = new string(Enumerable.Repeat(chars, 5)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
                isUnique = !await _db.User_Masters.AnyAsync(u => u.user_id == newId);
            } while (!isUnique);

            return newId;
        }

        private string GenerateTemporaryToken(RegisterStep1Dto data)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, data.UserName),
            new Claim(ClaimTypes.Email, data.Email),
            new Claim(ClaimTypes.StreetAddress, data.Address ?? ""),
            new Claim(ClaimTypes.MobilePhone, data.PhoneNumber ?? "")
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10), // Token ini hanya valid 10 menit
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}