using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using be_devextreme_starter.Services;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize]
    [Tags("Roles")]
    public class RoleController : ControllerBase
    {
        // Dependency injection: database context, audit service, and validator
        private readonly DataEntities _db;
        private readonly IAuditService _auditService;
        private readonly IValidator<RoleDto> _validator;

        public RoleController(DataEntities context, IAuditService auditService, IValidator<RoleDto> validator)
        {
            _db = context;
            _auditService = auditService;
            _validator = validator;
        }

        /// <summary>
        /// Get all active roles and their module codes.
        /// </summary>
        [HttpGet("get")]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            // Ambil semua role yang aktif (stsrc == "A")
            var roles = await _db.FW_Ref_Roles
                .Where(r => r.stsrc == "A")
                .Select(r => new RoleDto
                {
                    RoleId = r.role_id,
                    RoleCatatan = r.role_catatan,
                    // Ambil semua kode modul yang terkait dengan role ini
                    ModKodes = _db.FW_Role_Rights
                        .Where(c => c.role_id == r.role_id && c.stsrc == "A")
                        .Select(c => c.mod_kode).ToList()
                }).ToListAsync();

            // Return data dalam format yang bisa diproses DevExtreme
            return DataSourceLoader.Load(roles, loadOptions);
        }

        /// <summary>
        /// Get all modules for role assignment.
        /// </summary>
        [HttpGet("modules")]
        public async Task<object> GetModules(DataSourceLoadOptions loadOptions)
        {
            // Ambil semua modul, urutkan berdasarkan kode
            var modules = await _db.FW_Ref_Moduls
                .OrderBy(m => m.mod_kode)
                .ToListAsync();
            return DataSourceLoader.Load(modules, loadOptions);
        }

        /// <summary>
        /// Create a new role and its rights.
        /// </summary>
        [HttpPost("post")]
        public async Task<IActionResult> Post([FromForm] string values)
        {
            // Deserialize data dari form ke DTO
            var dto = JsonConvert.DeserializeObject<RoleDto>(values);
            // Validasi data menggunakan FluentValidation
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.BadRequest(validationResult.ToString(" ")));
            }

            // Buat objek role baru
            var newRole = new FW_Ref_Role
            {
                role_id = dto.RoleId,
                role_catatan = dto.RoleCatatan
            };
            // Set audit fields (created_by, date_created, dll)
            _auditService.SetStsrcFields(newRole);
            _db.FW_Ref_Roles.Add(newRole);

            // Tambahkan hak akses (rights) untuk setiap modul yang dipilih
            foreach (var modKode in dto.ModKodes)
            {
                var newRight = new FW_Role_Right
                {
                    role_id = newRole.role_id,
                    mod_kode = modKode,
                };
                _auditService.SetStsrcFields(newRight);
                _db.FW_Role_Rights.Add(newRight);
            }

            await _db.SaveChangesAsync();
            return Ok(ApiResponse<RoleDto>.Created(dto));
        }

        /// <summary>
        /// Update an existing role and its rights.
        /// </summary>
        [HttpPut("put")]
        public async Task<IActionResult> Put([FromForm] string key, [FromForm] string values)
        {
            // Cari role lama berdasarkan key
            var oldRole = await _db.FW_Ref_Roles.FindAsync(key);
            if (oldRole == null)
            {
                return NotFound(ApiResponse<object>.NotFound());
            }

            // Deserialize data baru
            var dto = JsonConvert.DeserializeObject<RoleDto>(values);
            // Update catatan role
            oldRole.role_catatan = dto.RoleCatatan;
            _auditService.SetStsrcFields(oldRole);

            // Hapus semua hak akses lama
            var existingRights = _db.FW_Role_Rights.Where(rr => rr.role_id == key);
            _db.FW_Role_Rights.RemoveRange(existingRights);

            // Tambahkan hak akses baru sesuai modul yang dipilih
            foreach (var modKode in dto.ModKodes)
            {
                var newRight = new FW_Role_Right
                {
                    role_id = key,
                    mod_kode = modKode,
                };
                _auditService.SetStsrcFields(newRight);
                _db.FW_Role_Rights.Add(newRight);
            }

            await _db.SaveChangesAsync();
            return Ok(ApiResponse<RoleDto>.Ok(dto));
        }

        /// <summary>
        /// Delete a role and its rights (soft delete).
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromForm] string key)
        {
            // Cari role berdasarkan key
            var role = await _db.FW_Ref_Roles.FindAsync(key);
            if (role == null)
            {
                return NotFound(ApiResponse<object>.NotFound());
            }

            // Soft delete role (ubah stsrc, bukan hard delete)
            _auditService.DeleteStsrc(role);

            // Soft delete semua hak akses yang terkait dengan role
            var rights = _db.FW_Role_Rights.Where(rr => rr.role_id == key);
            foreach (var right in rights)
            {
                _auditService.DeleteStsrc(right);
            }

            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(null, "Role deleted"));
        }
    }
}