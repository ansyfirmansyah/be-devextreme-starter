using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/outlets")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class OutletMasterApiController : Controller
    {
        #region
        private DataEntities _db;
        private IWebHostEnvironment _env;
        public OutletMasterApiController(DataEntities context, IWebHostEnvironment env)
        {
            this._db = context;
            this._env = env;
        }
        #endregion

        // GET: /api/outlets/get
        [HttpGet("get")]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var query = _db.Outlet_Masters
                .Where(field => field.stsrc == "A")
                .Select(s => new
                {
                    s.outlet_id,
                    s.outlet_kode,
                    s.outlet_nama,
                });
            return Ok(ApiResponse<object>.Ok(DataSourceLoader.Load(query, loadOptions)));
        }

        // INSERT (Untuk Tombol "Add")
        [HttpPost("post")]
        public async Task<IActionResult> Post([FromForm] string values)
        {
            var newOutlet = new Outlet_Master();
            JsonConvert.PopulateObject(values, newOutlet);
            if (checkDuplicateKode(newOutlet.outlet_kode, newOutlet.outlet_id))
            {
                return BadRequest(new { error = $"Kode '{newOutlet.outlet_kode}' sudah digunakan. Silakan gunakan kode lain." });
            }
            _db.SetStsrcFields(newOutlet);

            _db.Outlet_Masters.Add(newOutlet);
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<Outlet_Master>.Created(newOutlet));
        }

        // UPDATE (Untuk Tombol "Edit")
        [HttpPut("put")]
        public async Task<IActionResult> Put([FromForm] long key, [FromForm] string values)
        {
            var outlet = await _db.Outlet_Masters.FindAsync(key);
            if (outlet == null)
                return NotFound();

            JsonConvert.PopulateObject(values, outlet);
            if (checkDuplicateKode(outlet.outlet_kode, outlet.outlet_id))
            {
                return BadRequest(new { error = $"Kode '{outlet.outlet_kode}' sudah digunakan. Silakan gunakan kode lain." });
            }
            _db.SetStsrcFields(outlet);

            await _db.SaveChangesAsync();
            return Ok(ApiResponse<Outlet_Master>.Ok(outlet));
        }

        // DELETE (Untuk Tombol "Delete")
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromForm] long key)
        {
            var outlet = await _db.Outlet_Masters.FindAsync(key);
            if (outlet == null)
                return NotFound(ApiResponse<object>.NotFound());
            bool hasChildren = await _db.Sales_Masters.AnyAsync(s => s.outlet_id == key && s.stsrc == "A"); // periksa apakah masih memiliki sales
            if (hasChildren)
            {
                var resp = $"Outlet '{outlet.outlet_kode} - {outlet.outlet_nama}' tidak dapat dihapus karena masih memiliki data Sales";
                return BadRequest(new { error = resp });
            }

            _db.DeleteStsrc(outlet);
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(null, "data deleted"));
        }

        // GET: /api/outlets/getSalesForOutlet
        [HttpGet("getSalesForOutlet")]
        public object GetSalesForOutlet([FromQuery] int outletId, DataSourceLoadOptions loadOptions)
        {
            // Query ke database untuk mengambil data Sales
            // yang memiliki foreign key 'outlet_id' yang cocok.
            // Ganti 'Sales_Master' dan 'outlet_id' sesuai dengan nama tabel & kolom Anda.
            var salesQuery = _db.Sales_Masters
                .Where(s => s.outlet_id == outletId && s.stsrc == "A")
                .Select(s => new
                {
                    s.outlet_id,
                    s.sales_id,
                    s.sales_kode,
                    s.sales_nama
                });

            // DataSourceLoader akan menangani sisanya (paging, filter untuk detail grid jika ada)
            return Ok(ApiResponse<object>.Ok(DataSourceLoader.Load(salesQuery, loadOptions)));
        }

        [HttpGet("lookup")]
        public object GetOutletById(DataSourceLoadOptions loadOptions)
        {
            var query = _db.Outlet_Masters
                .Where(s => s.stsrc == "A")
                .OrderBy(s => s.outlet_nama)
                .Select(s => new
                {
                    s.outlet_id,
                    s.outlet_kode,
                    s.outlet_nama,
                    display = s.outlet_kode + " - " + s.outlet_nama
                });

            return DataSourceLoader.Load(query, loadOptions);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool checkDuplicateKode(string kode, long id)
        {
            // Jika id = 0 (mode Create)
            // Jika id > 0 (mode Edit)
            var isKodeExist = _db.Outlet_Masters
                                            .Any(x => x.outlet_kode == kode && x.outlet_id != id && x.stsrc == "A");
            return isKodeExist;
        }

        [HttpGet("ref/kode")]
        public object GetRefKodeOutlet(DataSourceLoadOptions loadOptions)
        {
            var query = from h in _db.Outlet_Masters
                        where h.stsrc == "A"
                        select new
                        {
                            h.outlet_kode,
                            display = h.outlet_kode + " - " + h.outlet_nama,
                        };
            return DataSourceLoader.Load(query, loadOptions);
        }
    }
}
