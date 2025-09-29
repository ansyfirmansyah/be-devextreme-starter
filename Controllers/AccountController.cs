using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using be_devextreme_starter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize]
    [Tags("Account")]
    public class AccountController : ControllerBase
    {
        private readonly DataEntities _db;
        private readonly IAuditService _auditService;

        public AccountController(DataEntities context, IAuditService auditService)
        {
            _db = context;
            _auditService = auditService;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            // Dapatkan User ID dari token yang sedang login
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(ApiResponse<object>.Error("Sesi tidak valid.", 401));
            }

            var user = await _db.User_Masters.FindAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User tidak ditemukan."));
            }

            // 3. Verifikasi password saat ini
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.user_password))
            {
                return BadRequest(ApiResponse<object>.BadRequest("Password saat ini salah."));
            }

            // 4. Hash dan update password baru
            user.user_password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.user_password_lastchange = System.DateTime.Now; // Update tanggal ganti password

            _auditService.SetStsrcFields(user); // Mengupdate modified_by dan date_modified
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Password berhasil diubah."));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Error("Sesi tidak valid.", 401));
            }

            var user = await _db.User_Masters.FindAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User tidak ditemukan."));
            }

            var profileDto = new UserProfileDto
            {
                UserName = user.user_nama,
                Email = user.user_email,
                Address = user.user_alamat,
                PhoneNumber = user.user_telp,
                Role = user.user_main_role,
                RegistrationDate = user.date_created,
                LastLoginDate = user.user_last_login,
                LastPasswordChangeDate = user.user_password_lastchange
            };

            return Ok(ApiResponse<UserProfileDto>.Ok(profileDto));
        }
    }
}
