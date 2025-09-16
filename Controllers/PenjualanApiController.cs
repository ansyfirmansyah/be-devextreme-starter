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
    }
}
