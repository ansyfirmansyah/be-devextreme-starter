using be_devextreme_starter.Data.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/penjualan")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class PenjualanApiController : Controller
    {
        #region
        private DataEntities _db;
        private IWebHostEnvironment _env;
        public PenjualanApiController(DataEntities context, IWebHostEnvironment env)
        {
            this._db = context;
            this._env = env;
        }
        #endregion

        // GET: /api/penjualan/get
        [HttpGet("get")]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var query = _db.Jual_Headers
                .Join(_db.Outlet_Masters, h => h.outlet_id, o => o.outlet_id, (h, o) => new { h, o })
                .Join(_db.Sales_Masters, c => c.h.sales_id, s => s.sales_id, (c, s) => new { c.h, c.o, s })
                .Where(c => c.h.stsrc == "A")
                .Select(c => new
                {
                    c.h.jualh_id,
                    c.h.jualh_kode,
                    c.h.jualh_date,
                    outlet_display = c.o.outlet_kode + " - " + c.o.outlet_nama,
                    sales_display = c.s.sales_kode + " - " + c.s.sales_nama,
                });
                return DataSourceLoader.Load(query, loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ref/kode")]
        public object GetRefKodePenjualan(DataSourceLoadOptions loadOptions)
        {
            var query = from h in _db.Jual_Headers
                        join o in _db.Outlet_Masters on h.outlet_id equals o.outlet_id
                        where h.stsrc == "A"
                        select new
                        {
                            h.jualh_kode,
                            display = h.jualh_kode + " - " + o.outlet_nama,
                        };
            return DataSourceLoader.Load(query, loadOptions);
        }
    }
}
