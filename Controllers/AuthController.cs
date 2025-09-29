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

            // Validate password
            var hashedPassword = ComputeSha256Hash(loginDto.Password);
            if (user.user_password != hashedPassword)
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
        /// Register endpoint: Creates a new user if UserId is not already taken.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _db.User_Masters.AnyAsync(u => u.user_id == registerDto.UserId))
            {
                return Conflict(ApiResponse<object>.Conflict("User ID sudah terdaftar."));
            }

            // Create new user object
            var newUser = new User_Master
            {
                user_id = registerDto.UserId,
                user_nama = registerDto.UserName,
                user_email = registerDto.Email,
                user_password = ComputeSha256Hash(registerDto.Password), // Hash password
                stsrc = "A",
                date_created = DateTime.Now,
                created_by = "SYSTEM_REGISTER"
            };

            _db.User_Masters.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Created(
                new { userId = newUser.user_id, userName = newUser.user_nama },
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

        /// <summary>
        /// Helper: Compute SHA256 hash for password.
        /// </summary>
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    // "x2" untuk format hexadecimal
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}