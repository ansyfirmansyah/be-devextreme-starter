using be_devextreme_starter.Data;
using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using be_devextreme_starter.Services;
using be_devextreme_starter.Validators;
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
    [Route("api/contacts")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    [Tags("Contacts")]
    public class ContactMasterApiController : ControllerBase
    {
        private readonly DataEntities _db;
        private readonly IAuditService _auditService;
        private readonly IValidator<ContactDTO> _validator;
        public ContactMasterApiController(DataEntities context, IAuditService auditService, IValidator<ContactDTO> validator)
        {
            _db = context;
            _auditService = auditService;
            _validator = validator;
        }

        /// <summary>
        /// Get all active contacts.
        /// </summary>
        [HttpGet("get")]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            // Ambil semua contact yang aktif (stsrc == "A")
            var query = await _db.Contact_Masters
                .Join(_db.Contact_Status_Masters, cm => cm.contact_status_id, csm => csm.contact_status_id, (con, csm) => new { con, csm })
                .Where(all => all.con.stsrc == "A")
                .Select(all => new {
                    all.con.contact_id,
                    all.con.full_name,
                    all.con.email,
                    all.con.company,
                    all.con.date_added,
                    all.csm.contact_status_name
                }).ToListAsync();

            // Return data dalam format yang bisa diproses DevExtreme
            return DataSourceLoader.Load(query, loadOptions);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetDetail(long id)
        {
            var contact = await _db.Contact_Masters
                .Where(cm => cm.contact_id == id && cm.stsrc == "A")
                .Select(cm => new
                {
                    cm.contact_id,
                    cm.full_name,
                    cm.email,
                    cm.phone_number,
                    cm.company,
                    cm.job_title,
                    cm.address,
                    cm.city.country_id,
                    cm.city_id,
                    cm.postal_code,
                    cm.date_added,
                    cm.last_contacted_date,
                    cm.lead_source_id,
                    cm.contact_status_id,
                    cm.estimated_value,
                    cm.is_subscribed,
                    cm.notes,
                })
                .FirstOrDefaultAsync();

            if (contact == null)
            {
                return NotFound(ApiResponse<object>.NotFound("Contact not found"));
            }

            return Ok(ApiResponse<object>.Ok(contact));
        }

        /// <summary>
        /// Get all country.
        /// </summary>
        [HttpGet("countries")]
        public async Task<object> GetCountries(DataSourceLoadOptions loadOptions)
        {
            // Ambil semua country, urutkan berdasarkan nama
            var query = await _db.Country_Masters
                .OrderBy(m => m.country_name)
                .ToListAsync();
            return DataSourceLoader.Load(query, loadOptions);
        }

        /// <summary>
        /// Get all city of current country.
        /// </summary>
        [HttpGet("cities")]
        public async Task<object> GetCities(DataSourceLoadOptions loadOptions, int countryId)
        {
            // Ambil semua city berdasarkan di country, urutkan berdasarkan nama
            var query = await _db.City_Masters
                .Where(cm => cm.country_id == countryId)
                .OrderBy(m => m.city_name)
                .ToListAsync();
            return DataSourceLoader.Load(query, loadOptions);
        }

        /// <summary>
        /// Get all lead source.
        /// </summary>
        [HttpGet("lead-sources")]
        public async Task<object> GetLeadSources(DataSourceLoadOptions loadOptions)
        {
            // Ambil semua lead source, urutkan berdasarkan nama
            var query = await _db.Lead_Source_Masters
                .OrderBy(m => m.lead_source_name)
                .ToListAsync();
            return DataSourceLoader.Load(query, loadOptions);
        }

        /// <summary>
        /// Get all status.
        /// </summary>
        [HttpGet("statuses")]
        public async Task<object> GetContactStatuses(DataSourceLoadOptions loadOptions)
        {
            // Ambil semua status kontak, urutkan berdasarkan nama
            var query = await _db.Contact_Status_Masters
                .OrderBy(m => m.contact_status_name)
                .ToListAsync();
            return DataSourceLoader.Load(query, loadOptions);
        }

        /// <summary>
        /// Create a new contact.
        /// </summary>
        [HttpPost("post")]
        [Authorize(Policy = "CanCreateContact")]
        public async Task<IActionResult> Post([FromForm] string values)
        {
            // Deserialize data dari form ke DTO
            var dto = JsonConvert.DeserializeObject<ContactDTO>(values);
            // Validasi data menggunakan FluentValidation
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.BadRequest(validationResult.ToString(" ")));
            }

            // Buat objek baru
            var newContact = new Contact_Master();
            WSMapper.CopyFieldValues(dto, newContact, "full_name,email,phone_number,company,job_title,address,city_id,postal_code,date_added,last_contacted_date,lead_source_id,contact_status_id,estimated_value,is_subscribed,notes");
            // Set audit fields (created_by, date_created, dll)
            _auditService.SetStsrcFields(newContact);
            _db.Contact_Masters.Add(newContact);

            await _db.SaveChangesAsync();
            return Ok(ApiResponse<ContactDTO>.Created(dto));
        }

        /// <summary>
        /// Update an existing contact.
        /// </summary>
        [HttpPut("put")]
        [Authorize(Policy = "CanEditContact")]
        public async Task<IActionResult> Put([FromForm] long key, [FromForm] string values)
        {
            // Cari role lama berdasarkan key
            var oldObj = await _db.Contact_Masters.FindAsync(key);
            if (oldObj == null)
            {
                return NotFound(ApiResponse<object>.NotFound());
            }

            var dto = new ContactDTO();
            JsonConvert.PopulateObject(values, dto);

            // Jalankan validasi secara manual
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.BadRequest(validationResult.ToString(" ")));
            }
            // fungsi untuk copy data dari object edit ke object di database, sebutkan kolom-kolom yang ingin diubah
            WSMapper.CopyFieldValues(dto, oldObj, "full_name,email,phone_number,company,job_title,address,city_id,postal_code,date_added,last_contacted_date,lead_source_id,contact_status_id,estimated_value,is_subscribed,notes");
            _auditService.SetStsrcFields(oldObj); // fungsi untuk mengisi stsrc, date_created, created_by, date_modified dan modified_by

            _db.SaveChanges(); // simpan perubahan ke database
            return Ok(ApiResponse<ContactDTO>.Ok(dto));
        }

        /// <summary>
        /// Delete a contact (soft delete).
        /// </summary>
        [HttpDelete("delete")]
        [Authorize(Policy = "CanDeleteContact")]
        public async Task<IActionResult> Delete([FromForm] long key)
        {
            // Cari role berdasarkan key
            var obj = await _db.Contact_Masters.FindAsync(key);
            if (obj == null)
            {
                return NotFound(ApiResponse<object>.NotFound());
            }

            // Soft delete role (ubah stsrc, bukan hard delete)
            _auditService.DeleteStsrc(obj);

            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(null, "Role deleted"));
        }
    }
}
