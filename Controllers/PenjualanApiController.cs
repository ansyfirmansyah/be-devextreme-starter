using be_devextreme_starter.Data;
using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using DevExpress.SpreadsheetSource.Implementation;
using DevExpress.XtraGauges.Core.Model;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/penjualan")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [Tags("Penjualan")]
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
                    c.o.outlet_id,
                    c.s.sales_id,
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

        // INSERT (Untuk Tombol "Add")
        [HttpPost("post")]
        public IActionResult Post([FromForm] string values)
        {
            try
            {
                // Gunakan DTO untuk mendapatkan Request Body, dikarenakan terdapat tambahan field seperti temptable_outlet_id
                var dto = JsonConvert.DeserializeObject<JualUpdateDto>(values);
                var obj = new Jual_Header();
                // Copy value dari DTO ke Object yang akan disimpan ke db, kecuali field tambahan
                WSMapper.CopyFieldValues(dto, obj, "jualh_kode,jualh_date,sales_id,outlet_id");
                if (checkDuplicateKode(dto.jualh_kode, dto.jualh_id))
                {
                    return BadRequest(new { message = $"Kode '{dto.jualh_kode}' sudah digunakan. Silakan gunakan kode lain." });
                }
                _db.SetStsrcFields(obj);
                _db.Jual_Headers.Add(obj);

                /* start input data detail ke DB (Add, Edit, Delete) */
                // Jual Detail
                var temp1 = new TempTableHelper<List<Jual_Detail>>(_db); // buat objek helper untuk menyimpan data sementara
                Guid temptableGuid1 = Guid.Parse(dto.temptable_detail_id); // ambil id temporary table dari object yang diisi ke tipe data Guid
                var detailList1 = temp1.GetContentAsObject(temptableGuid1); // ambil data dari temporary table berdasarkan id

                var tobeDeleteList = (from q in _db.Jual_Details where q.stsrc == "A" && q.jualh_id == obj.jualh_id select q).ToList(); // cari data yang akan dihapus
                foreach (var x in detailList1)
                {
                    Jual_Detail dataDetail = null; // inisialisasi object detail
                    if (x.juald_id > 0) // berarti sudah pernah disimpan ke db, lakukan update data
                    {
                        var query = (from q in tobeDeleteList where q.juald_id == x.juald_id select q);
                        if (query.Count() > 0) // jika data ditemukan, maka update data tersebut
                        {
                            dataDetail = query.First(); // ambil data yang akan diupdate
                            tobeDeleteList.Remove(dataDetail); // hapus dari list yang akan dihapus
                        }
                    }
                    else // berarti data baru, buat object baru
                    {
                        dataDetail = new Jual_Detail();
                        _db.Jual_Details.Add(dataDetail); // tambahkan ke db
                        dataDetail.jualh = obj; // set link ke object utama
                    }
                    dataDetail.barang_id = x.barang_id; // set data detailnya
                    dataDetail.juald_qty = x.juald_qty; // set data detailnya
                    dataDetail.juald_harga = x.juald_harga; // set data detailnya
                    dataDetail.juald_disk = x.juald_disk; // set data detailnya
                    _db.SetStsrcFields(dataDetail); // set kolom stsrc, date_created, created_by, date_modified dan modified_by
                }
                foreach (var x in tobeDeleteList) // hapus data yang sudah tidak ada di temporary table
                {
                    _db.DeleteStsrc(x); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)
                }
                /* end input data detail ke DB (Add, Edit, Delete) */

                _db.SaveChanges();
                return Ok(ApiResponse<object>.Created(dto));
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
                var oldObj = _db.Jual_Headers.Find(key); // cari terlebih dahulu data sesuai id yang diubah
                if (oldObj == null)
                    return NotFound(ApiResponse<object>.NotFound());
                var obj = JsonConvert.DeserializeObject<JualUpdateDto>(values);
                if (checkDuplicateKode(obj.jualh_kode, obj.jualh_id))
                {
                    return BadRequest(ApiResponse<object>.BadRequest($"Kode '{obj.jualh_kode}' sudah digunakan. Silakan gunakan kode lain."));
                }
                // fungsi untuk copy data dari object edit ke object di database, sebutkan kolom-kolom yang ingin diubah
                WSMapper.CopyFieldValues(obj, oldObj, "jualh_id,jualh_kode,jualh_date,sales_id,outlet_id");
                _db.SetStsrcFields(oldObj); // fungsi untuk mengisi stsrc, date_created, created_by, date_modified dan modified_by

                /* start input data detail ke DB (Add, Edit, Delete) */
                // Barang Outlet
                var temp1 = new TempTableHelper<List<JualDetailDTO>>(_db); // buat objek helper untuk menyimpan data sementara
                Guid temptableGuid1 = Guid.Parse(obj.temptable_detail_id); // ambil id temporary table dari object yang diisi ke tipe data Guid
                var detailList1 = temp1.GetContentAsObject(temptableGuid1); // ambil data dari temporary table berdasarkan id

                var tobeDeleteList = (from q in _db.Jual_Details where q.stsrc == "A" && q.jualh_id == obj.jualh_id select q).ToList(); // cari data yang akan dihapus
                foreach (var x in detailList1)
                {
                    Jual_Detail dataDetail = null; // inisialisasi object detail
                    if (x.juald_id > 0) // berarti sudah pernah disimpan ke db, lakukan update data
                    {
                        var query = (from q in tobeDeleteList where q.juald_id == x.juald_id select q);
                        if (query.Count() > 0) // jika data ditemukan, maka update data tersebut
                        {
                            dataDetail = query.First(); // ambil data yang akan diupdate
                            tobeDeleteList.Remove(dataDetail); // hapus dari list yang akan dihapus
                        }
                    }
                    else // berarti data baru, buat object baru
                    {
                        dataDetail = new Jual_Detail();
                        _db.Jual_Details.Add(dataDetail); // tambahkan ke db
                        dataDetail.jualh = oldObj; // set link ke object utama
                    }
                    dataDetail.barang_id = x.barang_id; // set data detailnya
                    dataDetail.juald_qty = x.juald_qty; // set data detailnya
                    dataDetail.juald_harga = x.juald_harga; // set data detailnya
                    dataDetail.juald_disk = x.juald_disk; // set data detailnya
                    _db.SetStsrcFields(dataDetail); // set kolom stsrc, date_created, created_by, date_modified dan modified_by
                }
                foreach (var x in tobeDeleteList) // hapus data yang sudah tidak ada di temporary table
                {
                    _db.DeleteStsrc(x); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)
                }

                /* end input data detail ke DB (Add, Edit, Delete) */

                _db.SaveChanges(); // simpan perubahan ke database
                return Ok(ApiResponse<JualUpdateDto>.Ok(obj));
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
            var obj = _db.Jual_Headers.Find(key); // cari data berdasarkan id yang diberikan
            _db.DeleteStsrc(obj); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)

            // Hapus data detail
            foreach (var detail in (obj.Jual_Details.Where(x => x.stsrc == "A")))
            {
                _db.DeleteStsrc(detail);
            }

            _db.SaveChanges(); // simpan perubahan ke database
            return Ok(ApiResponse<object>.Ok(null, "data deleted"));
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public bool checkDuplicateKode(string kode, long id)
        {
            // Jika id = 0 (mode Create)
            // Jika id > 0 (mode Edit)
            var isKodeExist = _db.Jual_Headers.Any(x => x.jualh_kode == kode && x.jualh_id != id && x.stsrc == "A");
            return isKodeExist;
        }

        [HttpGet("ref/outlet")]
        public object GetRefOutlet(DataSourceLoadOptions loadOptions)
        {
            var query = from o in _db.Outlet_Masters
                        where o.stsrc == "A"
                        select new
                        {
                            o.outlet_id,
                            display = o.outlet_kode + " - " + o.outlet_nama,
                        };
            return DataSourceLoader.Load(query, loadOptions);
        }

        [HttpGet("ref/sales")]
        public object GetRefSales(DataSourceLoadOptions loadOptions, long outlet_id)
        {
            var query = _db.Sales_Masters
                .Where(x => x.stsrc == "A" && x.outlet_id == outlet_id)
                .OrderBy(x => x.sales_nama)
                .Select(x => new
                {
                    x.sales_id,
                    x.sales_nama,
                    display = x.sales_kode + " - " + x.sales_nama
                });
            return DataSourceLoader.Load(query, loadOptions);
        }

        [HttpGet("ref/barang-diskon")]
        public object GetRefBarangDiskon(DataSourceLoadOptions loadOptions, long barang_id, int qty)
        {
            var query = _db.Barang_Diskons
                .Join(_db.Barang_Masters, bo => bo.barang_id, bm => bm.barang_id, (bo, bm) => new { bo, bm })
                .Where(c => c.bo.barang_id == barang_id && c.bo.stsrc == "A" && c.bm.stsrc == "A" && c.bo.barangd_qty < qty)
                .Select(c => new
                {
                    c.bo.barangd_id,
                    c.bo.barangd_qty,
                    c.bo.barangd_disc,
                    display = c.bo.barangd_qty + " @ " + c.bo.barangd_disc
                }).ToList();
            return DataSourceLoader.Load(query.ToList(), loadOptions);
        }

        [HttpGet("summary/jual-detail")]
        public object GetJualDetailWithHeaderId(long headerId, DataSourceLoadOptions loadOptions)
        {
            var detailData = _db.Jual_Details
                                .Where(s => s.jualh_id == headerId && s.stsrc == "A") // Filter berdasarkan ID dari master
                                .Select(s => new {
                                    s.juald_id,
                                    s.barang.barang_kode,
                                    s.barang.barang_nama,
                                    s.juald_qty,
                                    s.juald_harga,
                                    s.juald_disk
                                });

            return DataSourceLoader.Load(detailData, loadOptions);
        }

        [HttpGet("detail/jual-detail")]
        public object GetGridJualDetailTrans(DataSourceLoadOptions loadOptions, string temptable_detail_id)
        {
            Guid temptableGuid = Guid.Parse(temptable_detail_id); // konversi string ke Guid

            var temp = new TempTableHelper<List<JualDetailDTO>>(_db); // buat objek helper untuk menyimpan data sementara
            var details = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id
            return DataSourceLoader.Load(details, loadOptions);
        }

        [HttpPost("detail/init/jual-detail")]
        public IActionResult InitGridJualDetail([FromForm] long jualh_id)
        {
            try
            {
                var jualDetailDtoList = _db.Jual_Details
                .Join(_db.Barang_Masters, b => b.barang_id, k => k.barang_id, (b, k) => new { b, k })
                .Where(c => c.b.stsrc == "A" && c.b.jualh_id == jualh_id)
                .Select(c => new JualDetailDTO
                {
                    jualh_id = c.b.jualh_id,
                    juald_id = c.b.juald_id,
                    barang_id = c.b.barang_id,
                    juald_qty = c.b.juald_qty,
                    juald_harga = c.b.juald_harga,
                    juald_disk = c.b.juald_disk,
                    barang_kode = c.k.barang_kode,
                    barang_nama = c.k.barang_nama
                }).ToList();
                var temp1 = new TempTableHelper<List<JualDetailDTO>>(_db); // buat objek helper untuk menyimpan data sementara
                var tid1 = temp1.CreateContent(jualDetailDtoList); // buat temporary table dan simpan data ke dalamnya
                var result = new
                {
                    temptable_detail_id = tid1.ToString()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ref/barang")]
        public object GetRefBarang(DataSourceLoadOptions loadOptions, long outlet_id)
        {
            var query = _db.Barang_Outlets
                .Join(_db.Barang_Masters, bo => bo.barang_id, bm => bm.barang_id, (bo, bm) => new { bo, bm })
                .Where(c => c.bo.outlet_id == outlet_id && c.bo.stsrc == "A" && c.bm.stsrc == "A")
                .Select(c => new
                {
                    c.bm.barang_id,
                    c.bm.barang_kode,
                    c.bm.barang_nama,
                    c.bm.barang_harga,
                    display = c.bm.barang_kode + " - " + c.bm.barang_nama
                });
            return DataSourceLoader.Load(query.ToList(), loadOptions);
        }

        [HttpPost("detail/jual-detail")]
        public IActionResult AddDataJualDetail([FromForm] string barang_id, [FromForm] decimal juald_harga, [FromForm] int juald_qty, [FromForm] string? barangd_id, [FromForm] string temptable_detail_id)
        {
            Guid temptableGuid = Guid.Parse(temptable_detail_id); // konversi string ke Guid
            var temp = new TempTableHelper<List<JualDetailDTO>>(_db); // buat objek helper untuk menyimpan data sementara
            var listDetail = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id
            bool isDuplicate = listDetail.Any(v => v.barang_id.ToString() == barang_id);
            if (isDuplicate)
            {
                return BadRequest(new { message = "Barang tidak boleh duplikat!" });
            }
            // Hitung Diskon (jika ada)
            decimal juald_disk = 0;
            if (barangd_id != null && barangd_id != "null")
            {
                var barang_diskon = _db.Barang_Diskons.Where(bd => bd.barangd_id.ToString() == barangd_id && bd.stsrc == "A").First();
                if (barang_diskon == null)
                {
                    return BadRequest(new { error = "Diskon tidak ditemukan!" });
                }
                int countDisk = juald_qty / barang_diskon.barangd_qty.GetValueOrDefault();
                juald_disk = barang_diskon.barangd_disc.GetValueOrDefault() * countDisk;
            }

            var dataDetail = new JualDetailDTO();
            long minId = listDetail.OrderBy(x => x.juald_id).Select(x => Convert.ToInt64(x.juald_id)).FirstOrDefault(); // ambil id terkecil
            if (minId > 0) { minId = 0; }

            dataDetail.juald_id = minId - 1; // id baru akan menjadi id terkecil - 1
            dataDetail.barang_id = Convert.ToInt64(barang_id);
            dataDetail.barang_kode = _db.Barang_Masters.Find(dataDetail.barang_id)?.barang_kode;
            dataDetail.barang_nama = _db.Barang_Masters.Find(dataDetail.barang_id)?.barang_nama;
            dataDetail.juald_harga = juald_harga;
            dataDetail.juald_qty = juald_qty;
            dataDetail.juald_disk = juald_disk;

            listDetail.Add(dataDetail); // tambahkan data ke list
            temp.UpdateContent(temptableGuid, listDetail); // simpan kembali list ke temporary table

            var resp = "Data detail berhasil ditambahkan"; // pesan sukses
            return new JsonResult(resp); // mengembalikan response JSON dengan status sukses dan pesan
        }

        [HttpDelete("detail/jual-detail")]
        public IActionResult DeleteDataOutlet([FromForm] string key, [FromForm] string temptable_detail_id)
        {
            long id = Convert.ToInt64(key); // konversi key ke long
            Guid temptableGuid = Guid.Parse(temptable_detail_id.ToString()); // konversi string ke Guid
            var temp = new TempTableHelper<List<JualDetailDTO>>(_db); // buat objek helper untuk menyimpan data sementara
            var listDetail = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id

            var dataDetail = listDetail.Where(x => x.juald_id == id); // cari data detail berdasarkan id
            if (dataDetail.Count() > 0) // jika data ditemukan
            {
                listDetail.Remove(dataDetail.First()); // hapus data detail dari list
            }
            temp.UpdateContent(temptableGuid, listDetail); // simpan kembali list detail ke temporary table
            var resp = "Data detail berhasil dihapus"; // pesan sukses
            return new JsonResult(resp); // mengembalikan response JSON dengan status sukses dan pesan
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

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            // Mengatur nama file yang akan diunduh
            var fileName = "Template_Upload_Penjualan.xlsx";
            // Menggunakan MemoryStream agar tidak perlu menyimpan file fisik di server
            using (var stream = new MemoryStream())
            {
                // Membuat package Excel baru dengan EPPlus
                using (var package = new ExcelPackage(stream))
                {
                    /* === SHEET 1: HEADER === */
                    // Menambahkan worksheet (lembar kerja) baru
                    var headersheet = package.Workbook.Worksheets.Add("Header");
                    // --- Membuat Header Tabel ---
                    headersheet.Cells["A1"].Value = "Kode Penjualan";
                    headersheet.Cells["B1"].Value = "Tanggal Penjualan";
                    headersheet.Cells["C1"].Value = "Outlet";
                    headersheet.Cells["D1"].Value = "Sales";
                    // --- Styling Header (karena sebagai judul) ---
                    using (var range = headersheet.Cells["A1:D1"])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    // --- Menambahkan Contoh Data (Opsional, tapi sangat membantu pengguna) ---
                    headersheet.Cells["A2"].Value = "PJ-099";
                    headersheet.Cells["B2"].Value = new DateTime(2025, 8, 27);
                    headersheet.Cells["C2"].Value = "OK01 - Cilandak Town Square"; // Pastikan kode outlet contoh ini ada di database
                    headersheet.Cells["D2"].Value = "S05 - Fitri Hanif"; // Pastikan kode outlet contoh ini ada di database
                    headersheet.Cells["E2"].Value = "<--- Contoh pengisian data";
                    // Mengatur lebar kolom agar pas
                    headersheet.Column(1).AutoFit();
                    headersheet.Column(2).AutoFit();
                    headersheet.Column(3).AutoFit();
                    headersheet.Column(4).Width = 30;
                    headersheet.Column(2).Style.Numberformat.Format = "dd-MMM-yy";

                    /* === SHEET 2: DETAIL === */
                    // Menambahkan worksheet (lembar kerja) baru
                    var detailsheet = package.Workbook.Worksheets.Add("Detail");
                    // --- Membuat Header Tabel ---
                    detailsheet.Cells["A1"].Value = "Kode Penjualan";
                    detailsheet.Cells["B1"].Value = "Outlet";
                    detailsheet.Cells["C1"].Value = "Barang";
                    detailsheet.Cells["D1"].Value = "Harga";
                    detailsheet.Cells["E1"].Value = "Qty";
                    detailsheet.Cells["F1"].Value = "Diskon";
                    // --- Styling Header (karena sebagai judul) ---
                    using (var range = detailsheet.Cells["A1:F1"])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    // --- Menambahkan Contoh Data (Opsional, tapi sangat membantu pengguna) ---
                    detailsheet.Cells["A2"].Value = "PJ-099";
                    detailsheet.Cells["C2"].Value = "MB01 - Gulaku";
                    detailsheet.Cells["E2"].Value = 40;
                    detailsheet.Cells["F2"].Value = "20 @ 15000,00";
                    detailsheet.Cells["G2"].Value = "<--- Contoh pengisian data";
                    // Mengatur lebar kolom agar pas
                    detailsheet.Column(1).AutoFit();
                    detailsheet.Column(2).Width = 30;
                    detailsheet.Column(3).Width = 30;
                    detailsheet.Column(4).Width = 10;
                    detailsheet.Column(5).AutoFit();
                    detailsheet.Column(6).AutoFit();
                    detailsheet.Column(7).AutoFit();
                    // Mengatur format dan kunci kolom
                    detailsheet.Cells["B2:B30"].Style.Locked = true;
                    detailsheet.Column(4).Style.Numberformat.Format = "#,##0";
                    detailsheet.Cells["D2:D30"].Style.Locked = true;

                    /* === SHEET 3: REFERENSI === */
                    var referencesheet = package.Workbook.Worksheets.Add("Referensi");
                    // Referensi untuk Outlet dan Sales
                    var salesList = _db.Sales_Masters
                        .Join(_db.Outlet_Masters, s => s.outlet_id, o => o.outlet_id, (s, o) => new { s, o })
                        .Where(c => c.s.stsrc == "A" && c.o.stsrc == "A")
                        .OrderBy(c => c.o.outlet_nama)
                        .ThenBy(c => c.s.sales_nama)
                        .Select(c => new
                        {
                            c.o.outlet_id,
                            outlet = c.o.outlet_kode + " - " + c.o.outlet_nama,
                            c.s.sales_id,
                            sales = c.s.sales_kode + " - " + c.s.sales_nama,
                        }).ToList();
                    string lastOutlet = null;
                    int outletSeq = 1;
                    int salesSeq = 1;
                    int countSalesLastOutlet = 0;
                    foreach (var item in salesList)
                    {
                        if (item.outlet != lastOutlet)
                        {
                            if (lastOutlet != null)
                            {
                                var outletAName = "Sales_" + lastOutlet.Replace(" ", "_").Replace("-", "_");
                                package.Workbook.Names.Add(outletAName, referencesheet.Cells[salesSeq - countSalesLastOutlet, 2, salesSeq - 1, 2]);
                            }
                            referencesheet.Cells[outletSeq, 1].Value = item.outlet;
                            lastOutlet = item.outlet;
                            outletSeq++;
                            countSalesLastOutlet = 0;
                        }
                        referencesheet.Cells[salesSeq, 2].Value = item.sales;
                        salesSeq++;
                        countSalesLastOutlet++;
                    }
                    // Referensi untuk Barang
                    var barangList = _db.Barang_Outlets
                        .Join(_db.Outlet_Masters, bo => bo.outlet_id, o => o.outlet_id, (bo, o) => new { bo, o })
                        .Join(_db.Barang_Masters, c => c.bo.barang_id, bm => bm.barang_id, (c, bm) => new { c.bo, c.o, bm })
                        .Where(c => c.bo.stsrc == "A" && c.o.stsrc == "A" && c.bm.stsrc == "A")
                        .OrderBy(c => c.o.outlet_nama)
                        .ThenBy(c => c.bm.barang_nama)
                        .Select(c => new
                        {
                            c.o.outlet_id,
                            outlet = c.o.outlet_kode + " - " + c.o.outlet_nama,
                            c.bm.barang_id,
                            barang = c.bm.barang_kode + " - " + c.bm.barang_nama,
                            c.bm.barang_harga
                        }).ToList();
                    lastOutlet = null;
                    int barangSeq = 1;
                    int countBarangLastOutlet = 0;
                    foreach (var item in barangList)
                    {
                        if (item.outlet != lastOutlet)
                        {
                            if (lastOutlet != null)
                            {
                                var outletAName = "Barang_" + lastOutlet.Replace(" ", "_").Replace("-", "_");
                                package.Workbook.Names.Add(outletAName, referencesheet.Cells[barangSeq - countBarangLastOutlet, 3, barangSeq - 1, 3]);
                            }
                            lastOutlet = item.outlet;
                            countBarangLastOutlet = 0;
                        }
                        referencesheet.Cells[barangSeq, 3].Value = item.barang;
                        referencesheet.Cells[barangSeq, 4].Value = item.barang_harga;
                        barangSeq++;
                        countBarangLastOutlet++;
                    }
                    // Referensi untuk Barang Diskon
                    var barangDiscList = _db.Barang_Outlets
                        .Join(_db.Outlet_Masters, bo => bo.outlet_id, o => o.outlet_id, (bo, o) => new { bo, o })
                        .Join(_db.Barang_Masters, c => c.bo.barang_id, bm => bm.barang_id, (c, bm) => new { c.bo, c.o, bm })
                        .Join(_db.Barang_Diskons, c => c.bo.barang_id, bd => bd.barang_id, (c, bd) => new { c.bo, c.o, c.bm, bd })
                        .Where(c => c.bo.stsrc == "A" && c.o.stsrc == "A" && c.bm.stsrc == "A" && c.bd.stsrc == "A")
                        .OrderBy(c => c.o.outlet_nama)
                        .ThenBy(c => c.bm.barang_nama)
                        .ThenBy(c => c.bd.barangd_qty)
                        .Select(c => new
                        {
                            c.o.outlet_id,
                            outlet = c.o.outlet_kode + " - " + c.o.outlet_nama,
                            c.bm.barang_id,
                            barang = c.bm.barang_kode + " - " + c.bm.barang_nama,
                            diskon = c.bd.barangd_qty + " @ " + c.bd.barangd_disc
                        }).ToList();
                    string lastKey = null;
                    int barangDiscSeq = 1;
                    int countBarangDiscLastOutlet = 0;
                    foreach (var item in barangDiscList)
                    {
                        string key = item.outlet + "_" + item.barang;
                        if (key != lastKey)
                        {
                            if (lastKey != null)
                            {
                                var outletAName = "Diskon_" + lastKey.Replace(" ", "_").Replace("-", "_");
                                package.Workbook.Names.Add(outletAName, referencesheet.Cells[barangDiscSeq - countBarangDiscLastOutlet, 5, barangDiscSeq - 1, 5]);
                            }
                            lastKey = key;
                            countBarangDiscLastOutlet = 0;
                        }
                        referencesheet.Cells[barangDiscSeq, 5].Value = item.diskon;
                        barangDiscSeq++;
                        countBarangDiscLastOutlet++;
                    }
                    referencesheet.Column(1).AutoFit();
                    referencesheet.Column(2).AutoFit();
                    referencesheet.Column(3).AutoFit();
                    referencesheet.Column(4).AutoFit();
                    referencesheet.Column(5).AutoFit();

                    /* === KONFIGURASI DATA VALIDASI === */
                    // Validasi Outlet
                    int lastRowOutlet = checkLastRow(referencesheet, 1);
                    var outletValidation = headersheet.DataValidations.AddListValidation("C2:C10");
                    outletValidation.Formula.ExcelFormula = $"'{referencesheet.Name}'!$A$1:$A${lastRowOutlet}";
                    outletValidation.ShowErrorMessage = true;
                    outletValidation.ErrorTitle = "Outlet tidak valid";
                    outletValidation.Error = "Silakan pilih outlet dari dropdown list.";
                    // Validasi Sales
                    var salesValidation = headersheet.DataValidations.AddListValidation("D2:D10");
                    salesValidation.Formula.ExcelFormula = "=INDIRECT(\"Sales_\" & SUBSTITUTE(SUBSTITUTE(C2,\"-\",\"_\"),\" \",\"_\"))";
                    salesValidation.ShowErrorMessage = true;
                    salesValidation.ErrorTitle = "Sales tidak valid";
                    salesValidation.Error = "Silakan pilih sales dari dropdown list.";
                    // Validasi Barang
                    var barangValidation = detailsheet.DataValidations.AddListValidation("C2:C30");
                    barangValidation.Formula.ExcelFormula = "=INDIRECT(\"Barang_\" & SUBSTITUTE(SUBSTITUTE(B2,\"-\",\"_\"),\" \",\"_\"))";
                    barangValidation.ShowErrorMessage = true;
                    barangValidation.ErrorTitle = "Sales tidak valid";
                    barangValidation.Error = "Silakan pilih sales dari dropdown list.";
                    // Validasi Diskon Barang
                    var barangDiscValidation = detailsheet.DataValidations.AddListValidation("F2:F30");
                    barangDiscValidation.Formula.ExcelFormula = "=INDIRECT(\"Diskon_\" & SUBSTITUTE(SUBSTITUTE(B2&\"_\"&C2,\"-\",\"_\"),\" \",\"_\"))";
                    barangDiscValidation.ShowErrorMessage = true;
                    barangDiscValidation.ErrorTitle = "Sales tidak valid";
                    barangDiscValidation.Error = "Silakan pilih sales dari dropdown list.";

                    /* === LOOKUP DATA === */
                    int lastRowHargaRef = checkLastRow(referencesheet, 4);
                    for (int row = 2; row <= 30; row++)
                    {
                        var cellOutlet = detailsheet.Cells[row, 2];
                        cellOutlet.Formula = $"IF(A{row}<>\"\",VLOOKUP(A{row}, 'Header'!$A$2:$C$11, 3, 0),\"\")";
                        var cellHarga = detailsheet.Cells[row, 4];
                        cellHarga.Formula = $"IF(C{row}<>\"\",VLOOKUP(C{row}, 'Referensi'!$C$2:$D${lastRowHargaRef}, 2, 0),\"\")";
                    }

                    // Simpan perubahan ke package
                    headersheet.Select();
                    package.Save();
                }

                // Kembali ke awal stream sebelum dibaca untuk diunduh
                stream.Position = 0;

                // Mengembalikan file ke browser
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private int checkLastRow(ExcelWorksheet sheet, int column)
        {
            int lastRowInColumn = 0; // Default jika kolomnya ternyata kosong

            // Mulai dari baris paling bawah yang ada di sheet
            // Tanda tanya (?) adalah null-check untuk keamanan jika sheet benar-benar kosong
            int startRow = sheet.Dimension?.Rows ?? 1;

            for (int row = startRow; row >= 1; row--)
            {
                // Cek apakah sel di kolom A (indeks kolom 1) pada baris 'row' ini punya isi
                // Kita cek .Text agar bisa menangkap angka maupun tulisan
                var cellValue = sheet.Cells[row, column].Text;
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    lastRowInColumn = row; // Jika ada isi, simpan nomor barisnya
                    break; // Langsung keluar dari loop karena sudah ketemu yang paling bawah
                }
            }
            return lastRowInColumn;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private int getLastUsedRow(ExcelWorksheet sheet)
        {
            // Cek dulu apakah sheet-nya punya data atau tidak
            if (sheet.Dimension == null)
            {
                return 0; // Kembalikan 0 jika sheet kosong
            }

            int startRow = sheet.Dimension.Rows;
            int endCol = sheet.Dimension.Columns;

            // Loop dari baris paling BAWAH ke ATAS
            for (int row = startRow; row >= 1; row--)
            {
                // Di setiap baris, loop dari kolom pertama ke terakhir
                for (int col = 1; col <= endCol; col++)
                {
                    // Cek apakah sel di [baris, kolom] ini punya teks yang terlihat
                    var cellValue = sheet.Cells[row, col].Text;
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        // Jika ketemu satu saja sel berisi,
                        // berarti ini baris terakhir yang kita cari.
                        return row;
                    }
                }
            }

            // Jika loop selesai tanpa menemukan apa pun, berarti sheet kosong
            return 0;
        }

        [HttpPost("preview-upload")]
        public async Task<IActionResult> PreviewUpload([FromForm] FileUploadRequest request)
        {
            // Mendapatkan file dari request body
            var file = request.File;
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "File tidak boleh kosong." });
            }

            // Menggunakan DTO sebagai containernya
            var previewHeaderList = new List<JualhUploadPreviewDto>();
            var previewDetailList = new List<JualdUploadPreviewDto>();
            var kodes = new List<String>(); // untuk validasi duplikat kode di file yang sama
            int row = 1;

            // Menggunakan MemoryStream agar tidak perlu menyimpan file fisik di server
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                // Membuat package Excel baru dengan EPPlus
                using (var package = new ExcelPackage(stream))
                {
                    // Membaca sheet pertama
                    ExcelWorksheet headerWorksheet = package.Workbook.Worksheets[0];
                    ExcelWorksheet detailWorksheet = package.Workbook.Worksheets[1];
                    if (headerWorksheet.Dimension == null)
                    {
                        return BadRequest(new { message = "File tidak boleh kosong." });
                    }
                    // Menghitung isi baris
                    var rowCountHeader = headerWorksheet.Dimension.Rows;
                    var rowCountDetail = getLastUsedRow(detailWorksheet);

                    // Validasi judul kolom di header
                    if (headerWorksheet.Cells["A1"].Value?.ToString().Trim().ToLower() != "kode penjualan"
                        || headerWorksheet.Cells["B1"].Value?.ToString().Trim().ToLower() != "tanggal penjualan"
                        || headerWorksheet.Cells["C1"].Value?.ToString().Trim().ToLower() != "outlet"
                        || headerWorksheet.Cells["D1"].Value?.ToString().Trim().ToLower() != "sales"
                        )
                    {
                        return BadRequest(new { message = "Judul kolom di sheet Header tidak sesuai." });
                    }
                    // Validasi judul kolom di detail
                    if (detailWorksheet.Cells["A1"].Value?.ToString().Trim().ToLower() != "kode penjualan"
                        || detailWorksheet.Cells["B1"].Value?.ToString().Trim().ToLower() != "outlet"
                        || detailWorksheet.Cells["C1"].Value?.ToString().Trim().ToLower() != "barang"
                        || detailWorksheet.Cells["D1"].Value?.ToString().Trim().ToLower() != "harga"
                        || detailWorksheet.Cells["E1"].Value?.ToString().Trim().ToLower() != "qty"
                        || detailWorksheet.Cells["F1"].Value?.ToString().Trim().ToLower() != "diskon"
                        )
                    {
                        return BadRequest(new { message = "Judul kolom di sheet Detail tidak sesuai." });
                    }

                    // Mengolah isi sheet Header
                    for (row = 2; row <= rowCountHeader; row++)
                    {
                        var previewHeaderDto = new JualhUploadPreviewDto
                        {
                            RowNumber = row - 1, // row number akan digunakan sebagai nomor, oleh karena itu dikurang 1 dari index
                            Jualh_kode = headerWorksheet.Cells[row, 1].Value?.ToString().Trim(),
                            Outlet = headerWorksheet.Cells[row, 3].Value?.ToString().Trim(),
                            Sales = headerWorksheet.Cells[row, 4].Value?.ToString().Trim(),
                            IsValid = true
                        };
                        String[] outletChar = previewHeaderDto.Outlet?.Split(" - ");
                        previewHeaderDto.Outlet_kode = outletChar?.Length > 1 ? outletChar[0] : "";
                        previewHeaderDto.Outlet_nama = outletChar?.Length > 1 ? outletChar[1] : "";
                        String[] salesChar = previewHeaderDto.Sales?.Split(" - ");
                        previewHeaderDto.Sales_kode = salesChar?.Length > 1 ? salesChar[0] : "";
                        previewHeaderDto.Sales_nama = salesChar?.Length > 1 ? salesChar[1] : "";
                        object tanggalPenjualan = headerWorksheet.Cells[row, 2].Value;
                        List<String> errorMessages = new List<String>();

                        /* === Start Validation Header === */
                        if (string.IsNullOrEmpty(previewHeaderDto.Jualh_kode))
                        {
                            previewHeaderDto.IsValid = false;
                            errorMessages.Add("Kode Penjualan harus diisi.");
                        }
                        else
                        {
                            // cek kode duplikat
                            if (kodes.Any(a => a == previewHeaderDto.Jualh_kode))
                            {
                                previewHeaderDto.IsValid = false;
                                errorMessages.Add($"Kode Penjualan '{previewHeaderDto.Jualh_kode}' sudah ada di file yang diupload.");
                            }
                            // add existing kode untuk validasi
                            kodes.Add(previewHeaderDto.Jualh_kode);
                            if (_db.Jual_Headers.Any(s => s.jualh_kode == previewHeaderDto.Jualh_kode && s.stsrc == "A"))
                            {
                                previewHeaderDto.IsValid = false;
                                errorMessages.Add($"Kode Penjualan '{previewHeaderDto.Jualh_kode}' sudah ada di database.");
                            }
                        }

                        bool isTanggalValid = false;
                        if (tanggalPenjualan == null)
                        {
                            previewHeaderDto.IsValid = false;
                            errorMessages.Add("Tanggal Penjualan harus diisi.");
                            previewHeaderDto.Jualh_date = null;
                        } else
                        {
                            if (tanggalPenjualan is double)
                            {
                                // Konversi angka serial Excel menjadi DateTime
                                previewHeaderDto.Jualh_date = DateTime.FromOADate((double) tanggalPenjualan);
                                isTanggalValid = true;
                            }
                            // Terkadang EPPlus sudah pintar dan langsung mengubahnya menjadi DateTime
                            else if (tanggalPenjualan is DateTime)
                            {
                                previewHeaderDto.Jualh_date = (DateTime) tanggalPenjualan;
                                isTanggalValid = true;
                            }
                        }
                        if (tanggalPenjualan != null && !isTanggalValid)
                        {
                            previewHeaderDto.IsValid = false;
                            errorMessages.Add("Tanggal Penjualan tidak valid.");
                        }

                        if (string.IsNullOrEmpty(previewHeaderDto.Outlet))
                        {
                            previewHeaderDto.IsValid = false;
                            errorMessages.Add("Outlet harus diisi.");
                        } else if (string.IsNullOrEmpty(previewHeaderDto.Outlet_kode))
                        {
                            previewHeaderDto.IsValid = false;
                            errorMessages.Add("Outlet tidak valid.");
                        }
                        else
                        {
                            var outletCheck = _db.Outlet_Masters.FirstOrDefault(s => s.outlet_kode == previewHeaderDto.Outlet_kode && s.stsrc == "A");
                            if (outletCheck == null)
                            {
                                previewHeaderDto.IsValid = false;
                                errorMessages.Add($"Kode outlet '{previewHeaderDto.Outlet_kode}' tidak ditemukan.");
                            } else
                            {
                                previewHeaderDto.Outlet_id = outletCheck.outlet_id;
                            }
                        }

                        if (string.IsNullOrEmpty(previewHeaderDto.Sales))
                        {
                            previewHeaderDto.IsValid = false;
                            errorMessages.Add("Sales harus diisi.");
                        }
                        else if (string.IsNullOrEmpty(previewHeaderDto.Sales_kode))
                        {
                            previewHeaderDto.IsValid = false;
                            errorMessages.Add("Sales tidak valid.");
                        }
                        else
                        {
                            var salesCheck = _db.Sales_Masters.FirstOrDefault(s => s.sales_kode == previewHeaderDto.Sales_kode && s.stsrc == "A");
                            if (salesCheck == null)
                            {
                                previewHeaderDto.IsValid = false;
                                errorMessages.Add($"Kode sales '{previewHeaderDto.Sales_kode}' tidak ditemukan.");
                            }
                            else
                            {
                                previewHeaderDto.Sales_id = salesCheck.sales_id;
                            }
                        }

                        if (previewHeaderDto.Outlet_id != null && previewHeaderDto.Sales_id != null)
                        {
                            if (!_db.Sales_Masters.Any(s => s.outlet_id == previewHeaderDto.Outlet_id && s.sales_id == previewHeaderDto.Sales_id && s.stsrc == "A"))
                            {
                                previewHeaderDto.IsValid = false;
                                errorMessages.Add($"Sales '{previewHeaderDto.Sales}' tidak ada di Outlet '{previewHeaderDto.Outlet}'.");
                            }
                        }

                        // Kombinasikan error message
                        if (!previewHeaderDto.IsValid)
                        {
                            foreach (var item in errorMessages)
                            {
                                previewHeaderDto.ValidationMessage += (item + " ");
                            }
                        }

                        previewHeaderList.Add(previewHeaderDto);
                    }

                    // Mengolah isi sheet Detail
                    for (row = 2; row <= rowCountDetail; row++)
                    {
                        var previewDetailDto = new JualdUploadPreviewDto
                        {
                            RowNumber = row - 1, // row number akan digunakan sebagai nomor, oleh karena itu dikurang 1 dari index
                            Jualh_kode = detailWorksheet.Cells[row, 1].Value?.ToString().Trim(),
                            Barang = detailWorksheet.Cells[row, 3].Value?.ToString().Trim(),
                            Juald_harga = Convert.ToDecimal(detailWorksheet.Cells[row, 4].Value),
                            Juald_qty = Convert.ToInt32(detailWorksheet.Cells[row, 5].Value),
                            Diskon = detailWorksheet.Cells[row, 6].Value?.ToString().Trim(),
                            IsValid = true
                        };
                        String[] barangChar = previewDetailDto.Barang?.Split(" - ");
                        previewDetailDto.Barang_kode = barangChar?.Length > 1 ? barangChar[0] : "";
                        previewDetailDto.Barang_nama = barangChar?.Length > 1 ? barangChar[1] : "";
                        JualhUploadPreviewDto? headerDto = null;
                        List<String> errorMessages = new List<String>();

                        /* === Start Validation Detail === */
                        if (string.IsNullOrEmpty(previewDetailDto.Jualh_kode))
                        {
                            previewDetailDto.IsValid = false;
                            errorMessages.Add("Kode Penjualan harus diisi.");
                        }
                        else
                        {
                            headerDto = previewHeaderList.FirstOrDefault(s => s.Jualh_kode == previewDetailDto.Jualh_kode);
                            if (headerDto == null)
                            {
                                previewDetailDto.IsValid = false;
                                errorMessages.Add($"Kode Penjualan '{previewDetailDto.Jualh_kode}' tidak ada di sheet Header.");
                            } else if (!headerDto.IsValid)
                            {
                                previewDetailDto.IsValid = false;
                                errorMessages.Add($"Data Header tidak valid.");
                            }
                        }

                        if (string.IsNullOrEmpty(previewDetailDto.Barang))
                        {
                            previewDetailDto.IsValid = false;
                            errorMessages.Add("Barang harus diisi.");
                        }
                        else
                        {
                            var barangCheck = _db.Barang_Masters.FirstOrDefault(s => s.barang_kode == previewDetailDto.Barang_kode && s.stsrc == "A");
                            if (barangCheck == null)
                            {
                                previewDetailDto.IsValid = false;
                                errorMessages.Add($"Kode barang '{previewDetailDto.Barang_kode}' tidak ditemukan.");
                            }
                            else
                            {
                                previewDetailDto.Barang_id = barangCheck.barang_id;
                            }
                        }
                        if (previewDetailDto.Barang_id != null && headerDto?.Outlet_id != null)
                        {
                            if (!_db.Barang_Outlets.Any(s => s.outlet_id == headerDto.Outlet_id && s.barang_id == previewDetailDto.Barang_id && s.stsrc == "A"))
                            {
                                previewDetailDto.IsValid = false;
                                errorMessages.Add($"Barang '{previewDetailDto.Barang}' tidak ada di Outlet '{headerDto.Outlet}'.");
                            }
                        }
                        if (previewDetailDto.Juald_harga <= 0)
                        {
                            previewDetailDto.IsValid = false;
                            errorMessages.Add($"Harga harus > 0.");
                        }
                        if (previewDetailDto.Juald_qty <= 0)
                        {
                            previewDetailDto.IsValid = false;
                            errorMessages.Add($"Qty harus > 0.");
                        }
                        if (!string.IsNullOrEmpty(previewDetailDto.Diskon))
                        {
                            int discQty = Convert.ToInt32(previewDetailDto.Diskon.Split(" @ ")[0]);
                            if (discQty <= 0)
                            {
                                previewDetailDto.IsValid = false;
                                errorMessages.Add($"Qty Diskon harus > 0.");
                            } else if (previewDetailDto.Barang_id != null)
                            {
                                var diskonCheck = _db.Barang_Diskons.FirstOrDefault(s => s.barangd_qty == discQty && s.barang_id == previewDetailDto.Barang_id && s.stsrc == "A");
                                if (diskonCheck == null)
                                {
                                    previewDetailDto.IsValid = false;
                                    errorMessages.Add($"Diskon '{previewDetailDto.Diskon}' tidak ditemukan di barang terkait.");
                                } else if (discQty > previewDetailDto.Juald_qty)
                                {
                                    previewDetailDto.IsValid = false;
                                    errorMessages.Add($"Qty pembelian belum memenuhi minimum Qty diskon.");
                                }
                                else
                                {
                                    int countDisk = previewDetailDto.Juald_qty / diskonCheck.barangd_qty.GetValueOrDefault();
                                    previewDetailDto.Juald_disk = diskonCheck.barangd_disc.GetValueOrDefault() * countDisk;
                                }
                            }
                        }

                        // Kombinasikan error message
                        if (!previewDetailDto.IsValid)
                        {
                            foreach (var item in errorMessages)
                            {
                                previewDetailDto.ValidationMessage += (item + " ");
                            }
                        }

                        previewDetailList.Add(previewDetailDto);
                    }
                }
            }
            PenjualanUploadPreviewDto result = new PenjualanUploadPreviewDto();
            result.Header = previewHeaderList;
            result.Detail = previewDetailList;

            return Ok(ApiResponse<PenjualanUploadPreviewDto>.Ok(result));
        }

        [HttpPost("commit-upload")]
        public async Task<IActionResult> CommitUpload([FromBody] PenjualanUploadPreviewDto validPenjualan)
        {
            // Periksa isi request body
            if (validPenjualan == null || !validPenjualan.Header.Any())
            {
                return BadRequest(new { message = "Tidak ada data valid untuk diimpor." });
            }
            var headerToCreate = new List<Jual_Header>();
            var detailToCreate = new List<Jual_Detail>();

            // Insert ke database menggunakan transaction agar menjadi atomic (semua berhasil atau semua gagal total)
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    // Looping header
                    foreach (var header in validPenjualan.Header)
                    {
                        // Verifikasi Outlet
                        var outlet = _db.Outlet_Masters.FirstOrDefault(o => o.outlet_kode == header.Outlet_kode && o.stsrc == "A");
                        if (outlet == null || outlet.outlet_id != header.Outlet_id)
                        {
                            // Jika outlet tidak ditemukan, lemparkan error yang lebih jelas
                            return BadRequest(new { message = $"Gagal menyimpan: Outlet dengan kode '{header.Outlet_kode}' tidak ditemukan di database." });
                        }
                        // Verifikasi Sales
                        var sales = _db.Sales_Masters.FirstOrDefault(o => o.sales_kode == header.Sales_kode && o.stsrc == "A");
                        if (outlet == null || sales.sales_id != header.Sales_id)
                        {
                            // Jika outlet tidak ditemukan, lemparkan error yang lebih jelas
                            return BadRequest(new { message = $"Gagal menyimpan: Sales dengan kode '{header.Sales_kode}' tidak ditemukan di database." });
                        }
                        // Init data sesuai kebutuhan database
                        var newData = new Jual_Header
                        {
                            jualh_kode = header.Jualh_kode,
                            jualh_date = header.Jualh_date.GetValueOrDefault(),
                            sales_id = header.Sales_id.GetValueOrDefault(),
                            outlet_id = header.Outlet_id.GetValueOrDefault(),
                        };
                        _db.SetStsrcFields(newData);
                        headerToCreate.Add(newData);
                    }

                    // Looping detail
                    foreach (var detail in validPenjualan.Detail)
                    {
                        // Verifikasi Barang
                        var barang = _db.Barang_Masters.FirstOrDefault(o => o.barang_kode == detail.Barang_kode && o.stsrc == "A");
                        if (barang == null || barang.barang_id != detail.Barang_id)
                        {
                            // Jika outlet tidak ditemukan, lemparkan error yang lebih jelas
                            return BadRequest(new { message = $"Gagal menyimpan: Barang dengan kode '{detail.Barang_kode}' tidak ditemukan di database." });
                        }
                        // Verifikasi Diskon
                        if (detail.Barangd_id != null)
                        {
                            var diskon = _db.Barang_Diskons.FirstOrDefault(o => o.barangd_id == detail.Barangd_id && o.stsrc == "A");
                            if (diskon == null)
                            {
                                // Jika outlet tidak ditemukan, lemparkan error yang lebih jelas
                                return BadRequest(new { message = $"Gagal menyimpan: Diskon dengan qty '{detail.Juald_qty}' tidak ditemukan di database." });
                            }
                        }
                        // Verifikasi Header
                        var header = headerToCreate.FirstOrDefault(h => h.jualh_kode == detail.Jualh_kode);
                        if (header == null)
                        {
                            return BadRequest(new { message = $"Gagal menyimpan: Header Penjualan dengan kode '{detail.Jualh_kode}' tidak ditemukan." });
                        }
                        // Init data sesuai kebutuhan database
                        var newData = new Jual_Detail
                        {
                            jualh = header,
                            barang = barang,
                            juald_harga = detail.Juald_harga,
                            juald_qty = detail.Juald_qty,
                            juald_disk = detail.Juald_disk.GetValueOrDefault()
                        };
                        _db.SetStsrcFields(newData);
                        detailToCreate.Add(newData);
                    }

                    await _db.Jual_Headers.AddRangeAsync(headerToCreate);
                    await _db.Jual_Details.AddRangeAsync(detailToCreate);
                    await _db.SaveChangesAsync();

                    // Jika semua sukses, commit transaksi
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Jika ada satu saja error, batalkan semua yang sudah dilakukan
                    await transaction.RollbackAsync();
                    // Kembalikan pesan error
                    return StatusCode(500, new { message = "Terjadi kesalahan saat menyimpan ke database. Semua data dibatalkan.", error = ex.Message });
                }
            }

            string respMsg = $"{headerToCreate.Count} data header berhasil diimpor.";
            if (detailToCreate.Count > 0)
            {
                respMsg += $" {detailToCreate.Count} data detail berhasil diimpor.";
            }
            return Ok(new { message = respMsg });
        }
    }
}
