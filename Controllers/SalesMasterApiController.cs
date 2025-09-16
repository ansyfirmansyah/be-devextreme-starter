using be_devextreme_starter.Data;
using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/sales")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class SalesMasterApiController : Controller
    {
        #region
        private DataEntities _db;
        private IWebHostEnvironment _env;
        public SalesMasterApiController(DataEntities context, IWebHostEnvironment env)
        {
            this._db = context;
            this._env = env;
        }
        #endregion

        // GET: /api/sales/get
        [HttpGet("get")]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var query = _db.Sales_Masters
                .Where(field => field.stsrc == "A")
                .Select(s => new
                {
                    s.sales_id,
                    s.sales_kode,
                    s.sales_nama,
                    s.outlet_id,
                    outlet_display = s.outlet.outlet_kode + " - " + s.outlet.outlet_nama
                });
                return DataSourceLoader.Load(query, loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // INSERT (Untuk Tombol "Add")
        [HttpPost("post")]
        public IActionResult Post([FromForm] string values)
        {
            try
            {
                var newData = new Sales_Master();
                JsonConvert.PopulateObject(values, newData);
                if (checkDuplicateKode(newData.sales_kode, newData.sales_id))
                {
                    return BadRequest(new { message = $"Kode '{newData.sales_kode}' sudah digunakan. Silakan gunakan kode lain." });
                }
                _db.SetStsrcFields(newData);

                _db.Sales_Masters.Add(newData);
                _db.SaveChanges();
                return Ok(ApiResponse<Sales_Master>.Created(newData));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // UPDATE (Untuk Tombol "Edit")
        [HttpPut("put")]
        public IActionResult Put([FromForm] long key, [FromForm] string values)
        {
            try
            {
                var oldObj = _db.Sales_Masters.Find(key); // cari terlebih dahulu data sesuai id yang diubah
                if (oldObj == null)
                    return NotFound();
                var dto = JsonConvert.DeserializeObject<SalesUpdateDto>(values);
                if (checkDuplicateKode(dto.sales_kode, dto.sales_id))
                {
                    return BadRequest(new { message = $"Kode '{dto.sales_kode}' sudah digunakan. Silakan gunakan kode lain." });
                }
                // fungsi untuk copy data dari object edit ke object di database, sebutkan kolom-kolom yang ingin diubah
                WSMapper.CopyFieldValues(dto, oldObj, "sales_id,sales_kode,sales_nama,outlet_id");
                _db.SetStsrcFields(oldObj); // fungsi untuk mengisi stsrc, date_created, created_by, date_modified dan modified_by

                _db.SaveChanges(); // simpan perubahan ke database
                return Ok(ApiResponse<SalesUpdateDto>.Ok(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE (Untuk Tombol "Delete")
        [HttpDelete("delete")]
        public IActionResult Delete([FromForm] long key)
        {
            try
            {
                var existingData = _db.Sales_Masters.Find(key);
                if (existingData == null)
                    return NotFound();
                bool hasChildren = _db.Jual_Headers.Any(s => s.sales_id == key && s.stsrc == "A"); // periksa apakah masih memiliki transaksi
                if (hasChildren)
                {
                    var resp = $"Sales '{existingData.sales_kode} - {existingData.sales_nama}' tidak dapat dihapus karena masih memiliki transaksi";
                    return BadRequest(new { message = resp });
                }

                _db.DeleteStsrc(existingData);
                _db.SaveChanges();
                return Ok(ApiResponse<object>.Ok(null, "data deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool checkDuplicateKode(string kode, long id)
        {
            // Jika id = 0 (mode Create)
            // Jika id > 0 (mode Edit)
            var isKodeExist = _db.Sales_Masters
                                            .Any(x => x.sales_kode == kode && x.sales_id != id && x.stsrc == "A");
            return isKodeExist;
        }
    }
}
