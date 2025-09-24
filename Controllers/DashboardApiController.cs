using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [Tags("Dashboard")]
    public class DashboardApiController : Controller
    {
        // Dependency injection for database context and environment
        private readonly DataEntities _db;
        private readonly IWebHostEnvironment _env;

        public DashboardApiController(DataEntities context, IWebHostEnvironment env)
        {
            _db = context;
            _env = env;
        }

        /// <summary>
        /// Get KPI summary for the current month.
        /// </summary>
        /// <returns>KPI data including sales, transactions, top products, outlets, and sales.</returns>
        [HttpGet("kpi")]
        public object Get()
        {
            try
            {
                var response = new KpiResponseDto();

                // Calculate first and last day of the current month
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // Get all valid header transactions for the current month
                var headerTransactionOfCurrentMonth = _db.Jual_Headers
                    .Join(_db.Outlet_Masters, h => h.outlet_id, o => o.outlet_id, (h, o) => new { h, o })
                    .Join(_db.Sales_Masters, c => c.h.sales_id, s => s.sales_id, (c, s) => new { c.h, c.o, s })
                    .Where(c => c.h.stsrc == "A" && c.o.stsrc == "A" && c.s.stsrc == "A"
                        && c.h.jualh_date >= firstDayOfMonth && c.h.jualh_date <= lastDayOfMonth)
                    .Select(c => new
                    {
                        c.h.jualh_kode,
                        c.h.jualh_date,
                        c.o.outlet_nama,
                        c.s.sales_nama
                    })
                    .ToList();

                // Get all valid detail transactions for the current month
                var detailTransactionOfCurrentMonth = _db.Jual_Details
                    .Join(_db.Jual_Headers, d => d.jualh_id, h => h.jualh_id, (d, h) => new { d, h })
                    .Join(_db.Barang_Masters, c => c.d.barang_id, b => b.barang_id, (c, b) => new { c.d, c.h, b })
                    .Where(c => c.d.stsrc == "A" && c.h.stsrc == "A" && c.b.stsrc == "A"
                        && c.h.jualh_date >= firstDayOfMonth && c.h.jualh_date <= lastDayOfMonth)
                    .Select(c => new
                    {
                        c.h.jualh_kode,
                        c.h.jualh_date,
                        c.d.juald_harga,
                        c.d.juald_qty,
                        c.d.juald_disk,
                        c.b.barang_nama,
                        total = (c.d.juald_harga * c.d.juald_qty) - c.d.juald_disk
                    })
                    .ToList();

                // Calculate total sales and transaction count for the month
                response.salesMonth = detailTransactionOfCurrentMonth.Sum(val => val.total);
                response.transactionsMonth = headerTransactionOfCurrentMonth.Count();

                // Get top 5 products by sales count (with more than 1 sale)
                var topProduct = detailTransactionOfCurrentMonth
                    .GroupBy(c => c.barang_nama)
                    .Select(g => new { Nama = g.Key, Count = g.Count() })
                    .Where(x => x.Count > 1)
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                response.topProducts = new List<string>();
                foreach (var item in topProduct)
                {
                    response.topProducts.Add($"{item.Nama} ({item.Count})");
                }

                // Get top 5 outlets by transaction count (with more than 1 transaction)
                var topOutlet = headerTransactionOfCurrentMonth
                    .GroupBy(c => c.outlet_nama)
                    .Select(g => new { Nama = g.Key, Count = g.Count() })
                    .Where(x => x.Count > 1)
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                response.topOutlets = new List<string>();
                foreach (var item in topOutlet)
                {
                    response.topOutlets.Add($"{item.Nama} ({item.Count})");
                }

                // Get top 5 sales by transaction count (with more than 1 transaction)
                var topSales = headerTransactionOfCurrentMonth
                    .GroupBy(c => c.sales_nama)
                    .Select(g => new { Nama = g.Key, Count = g.Count() })
                    .Where(x => x.Count > 1)
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                response.topSales = new List<string>();
                foreach (var item in topSales)
                {
                    response.topSales.Add($"{item.Nama} ({item.Count})");
                }

                // Return successful response
                return Ok(ApiResponse<KpiResponseDto>.Ok(response));
            }
            catch (Exception ex)
            {
                // Return error response if exception occurs
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get sales trend for the last 14 days.
        /// </summary>
        /// <param name="loadOptions">DevExtreme data loading options</param>
        /// <returns>Sales trend data for charting</returns>
        [HttpGet("sales-trend")]
        public object GetSalesTrend(DataSourceLoadOptions loadOptions)
        {
            // 1. Prepare date range for the last 14 days
            var today = DateTime.Today;
            var fourteenDaysAgo = today.AddDays(-13);
            var dateRange = Enumerable.Range(0, 14)
                .Select(i => fourteenDaysAgo.AddDays(i));

            // 2. Get all sales details within the date range
            var relevantSalesDetails = _db.Jual_Details
                .Join(_db.Jual_Headers, d => d.jualh_id, h => h.jualh_id, (d, h) => new { d, h })
                .Where(c => c.h.stsrc == "A" && c.d.stsrc == "A"
                    && c.h.jualh_date >= fourteenDaysAgo && c.h.jualh_date <= today);

            // 3. Group sales data by date in memory
            var salesDataByDate = relevantSalesDetails
                .ToList() // Load data into memory
                .GroupBy(c => c.h.jualh_date.Date) // Group by transaction date
                .Select(group => new
                {
                    Date = group.Key,
                    TotalSales = group.Sum(c => (c.d.juald_qty * c.d.juald_harga) - c.d.juald_disk)
                })
                .ToDictionary(g => g.Date, g => g.TotalSales); // For fast lookup

            // 4. Merge with date range to ensure days with no sales are included (value 0)
            var finalSalesTrend = dateRange
                .Select(date => new SalesTrendDto
                {
                    Date = date,
                    TotalSales = salesDataByDate.ContainsKey(date) ? salesDataByDate[date] : 0
                })
                .AsQueryable(); // Convert to IQueryable for DataSourceLoader

            // 5. Return data using DevExtreme's DataSourceLoader
            return DataSourceLoader.Load(finalSalesTrend, loadOptions);
        }
    }
}