using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using be_devextreme_starter.Services;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize] // Controller ini boleh diakses tanpa login
    [Tags("User")]
    public class UserController : ControllerBase
    {
        // Dependency injection for database context and configuration
        private readonly DataEntities _db;
        private readonly IConfiguration _config;
        private readonly IAuditService _auditService;

        public UserController(DataEntities context, IConfiguration config, IAuditService auditService)
        {
            _db = context;
            _config = config;
            _auditService = auditService;
        }

        /// <summary>
        /// Get all active user and their roles.
        /// </summary>
        [HttpGet("roles/get")]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            // Ambil semua user yang aktif (stsrc == "A")
            var roles = await _db.User_Masters
                .Where(r => r.stsrc == "A")
                .Select(r => new UserRoleDto
                {
                    userId = r.user_id,
                    userName = r.user_nama,
                    userEmail = r.user_email,
                    // Ambil semua kode modul yang terkait dengan role ini
                    roles = _db.FW_User_Roles
                        .Where(c => c.user_id == r.user_id && c.stsrc == "A")
                        .Select(c => c.role_id).ToList()
                }).ToListAsync();

            // Return data dalam format yang bisa diproses DevExtreme
            return DataSourceLoader.Load(roles, loadOptions);
        }

        /// <summary>
        /// Update an user and its roles.
        /// </summary>
        [HttpPut("roles/put")]
        [Authorize(Policy = "CanEditUserRole")]
        public async Task<IActionResult> Put([FromForm] string key, [FromForm] string values)
        {
            // Cari user berdasarkan key
            var user = await _db.User_Masters.FindAsync(key);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound());
            }

            // Deserialize data baru
            var dto = JsonConvert.DeserializeObject<UserRoleDto>(values);

            // Hapus semua hak akses lama
            var existingRoles = _db.FW_User_Roles.Where(rr => rr.user_id == key);
            _db.FW_User_Roles.RemoveRange(existingRoles);

            // Tambahkan hak akses baru sesuai modul yang dipilih
            foreach (var role in dto.roles)
            {
                var newRole = new FW_User_Role
                {
                    user_id = key,
                    role_id = role,
                };
                _auditService.SetStsrcFields(newRole);
                _db.FW_User_Roles.Add(newRole);
            }

            await _db.SaveChangesAsync();
            return Ok(ApiResponse<UserRoleDto>.Ok(dto));
        }
    }
}
