using be_devextreme_starter.Data;
using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using be_devextreme_starter.Services;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.CodeParser;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/barang")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    [Tags("Barang")]
    public class BarangMasterApiController : Controller
    {
        #region
        private DataEntities _db;
        private IWebHostEnvironment _env;
        private readonly IAuditService _auditService;
        public BarangMasterApiController(DataEntities context, IWebHostEnvironment env, IAuditService auditService)
        {
            this._db = context;
            this._env = env;
            this._auditService = auditService;
        }
        #endregion

        // GET: /api/barang/get
        [HttpGet("get")]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var query = _db.Barang_Masters
                .Join(_db.Klasifikasi_Masters, b => b.klas_id, k => k.klas_id, (b, k) => new { b, k })
                .Where(c => c.b.stsrc == "A")
                .Select(c => new
                {
                    c.b.barang_id,
                    c.b.barang_kode,
                    c.b.barang_nama,
                    c.b.barang_harga,
                    c.b.klas_id,
                    klasifikasi_display = c.k.klas_kode + " - " + c.k.klas_nama
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
        [Authorize(Policy = "CanCreateBarang")]
        public IActionResult Post([FromForm] string values)
        {
            try
            {
                // Gunakan DTO untuk mendapatkan Request Body, dikarenakan terdapat tambahan field seperti temptable_outlet_id
                var dto = JsonConvert.DeserializeObject<BarangMasterUpdateDto>(values);
                var obj = new Barang_Master();
                // Copy value dari DTO ke Object yang akan disimpan ke db, kecuali field tambahan
                WSMapper.CopyFieldValues(dto, obj, "barang_kode,barang_nama,barang_harga,klas_id");
                if (checkDuplicateKode(dto.barang_kode, dto.barang_id))
                {
                    return BadRequest(new { message = $"Kode '{dto.barang_kode}' sudah digunakan. Silakan gunakan kode lain." });
                }
                _auditService.SetStsrcFields(obj);
                _db.Barang_Masters.Add(obj);

                /* start input data detail ke DB (Add, Edit, Delete) */
                // Barang Outlet
                var temp1 = new TempTableHelper<List<Barang_Outlet>>(_db); // buat objek helper untuk menyimpan data sementara
                Guid temptableGuid1 = Guid.Parse(dto.temptable_outlet_id); // ambil id temporary table dari object yang diisi ke tipe data Guid
                var detailList1 = temp1.GetContentAsObject(temptableGuid1); // ambil data dari temporary table berdasarkan id

                var tobeDeleteList = (from q in _db.Barang_Outlets where q.stsrc == "A" && q.barang_id == obj.barang_id select q).ToList(); // cari data yang akan dihapus
                foreach (var x in detailList1)
                {
                    Barang_Outlet dataDetail = null; // inisialisasi object detail
                    if (x.barango_id > 0) // berarti sudah pernah disimpan ke db, lakukan update data
                    {
                        var query = (from q in tobeDeleteList where q.barango_id == x.barango_id select q);
                        if (query.Count() > 0) // jika data ditemukan, maka update data tersebut
                        {
                            dataDetail = query.First(); // ambil data yang akan diupdate
                            tobeDeleteList.Remove(dataDetail); // hapus dari list yang akan dihapus
                        }
                    }
                    else // berarti data baru, buat object baru
                    {
                        dataDetail = new Barang_Outlet();
                        _db.Barang_Outlets.Add(dataDetail); // tambahkan ke db
                        dataDetail.barang = obj; // set link ke object utama
                    }
                    dataDetail.outlet_id = x.outlet_id; // set data detailnya
                    _auditService.SetStsrcFields(dataDetail); // set kolom stsrc, date_created, created_by, date_modified dan modified_by
                }
                foreach (var x in tobeDeleteList) // hapus data yang sudah tidak ada di temporary table
                {
                    _auditService.DeleteStsrc(x); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)
                }

                // Barang Diskon
                var temp2 = new TempTableHelper<List<Barang_Diskon>>(_db); // buat objek helper untuk menyimpan data sementara
                Guid temptableGuid2 = Guid.Parse(dto.temptable_diskon_id); // ambil id temporary table dari object yang diisi ke tipe data Guid
                var detailList2 = temp2.GetContentAsObject(temptableGuid2); // ambil data dari temporary table berdasarkan id

                /* start input data detail ke DB (Add, Edit, Delete) */
                var tobeDeleteList2 = (from q in _db.Barang_Diskons where q.stsrc == "A" && q.barang_id == obj.barang_id select q).ToList(); // cari data yang akan dihapus
                foreach (var x in detailList2)
                {
                    Barang_Diskon dataDetail = null; // inisialisasi object detail
                    if (x.barangd_id > 0) // berarti sudah pernah disimpan ke db, lakukan update data
                    {
                        var query = (from q in tobeDeleteList2 where q.barangd_id == x.barangd_id select q);
                        if (query.Count() > 0) // jika data ditemukan, maka update data tersebut
                        {
                            dataDetail = query.First(); // ambil data yang akan diupdate
                            tobeDeleteList2.Remove(dataDetail); // hapus dari list yang akan dihapus
                        }
                    }
                    else // berarti data baru, buat object baru
                    {
                        dataDetail = new Barang_Diskon();
                        _db.Barang_Diskons.Add(dataDetail); // tambahkan ke db
                        dataDetail.barang = obj; // set link ke object utama
                    }
                    dataDetail.barangd_disc = x.barangd_disc; // set data detailnya
                    dataDetail.barangd_qty = x.barangd_qty; // set data detailnya
                    if (dataDetail.barangd_disc > (dataDetail.barangd_qty * obj.barang_harga))
                    {
                        return BadRequest(new { message = $"Nilai Diskon tidak boleh lebih besar dari harga" });
                    }

                    _auditService.SetStsrcFields(dataDetail); // set kolom stsrc, date_created, created_by, date_modified dan modified_by
                }
                foreach (var x in tobeDeleteList) // hapus data yang sudah tidak ada di temporary table
                {
                    _auditService.DeleteStsrc(x); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)
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
        [Authorize(Policy = "CanEditBarang")]
        public IActionResult Put([FromForm] long key, [FromForm] string values)
        {
            try
            {
                var oldObj = _db.Barang_Masters.Find(key); // cari terlebih dahulu data sesuai id yang diubah
                if (oldObj == null)
                    return NotFound();
                var obj = JsonConvert.DeserializeObject<BarangMasterUpdateDto>(values);
                if (checkDuplicateKode(obj.barang_kode, obj.barang_id))
                {
                    return BadRequest(new { message = $"Kode '{obj.barang_kode}' sudah digunakan. Silakan gunakan kode lain." });
                }
                // fungsi untuk copy data dari object edit ke object di database, sebutkan kolom-kolom yang ingin diubah
                WSMapper.CopyFieldValues(obj, oldObj, "barang_id,barang_kode,barang_nama,barang_harga,klas_id");
                _auditService.SetStsrcFields(oldObj); // fungsi untuk mengisi stsrc, date_created, created_by, date_modified dan modified_by

                /* start input data detail ke DB (Add, Edit, Delete) */
                // Barang Outlet
                var temp1 = new TempTableHelper<List<Barang_Outlet>>(_db); // buat objek helper untuk menyimpan data sementara
                Guid temptableGuid1 = Guid.Parse(obj.temptable_outlet_id); // ambil id temporary table dari object yang diisi ke tipe data Guid
                var detailList1 = temp1.GetContentAsObject(temptableGuid1); // ambil data dari temporary table berdasarkan id

                var tobeDeleteList = (from q in _db.Barang_Outlets where q.stsrc == "A" && q.barang_id == obj.barang_id select q).ToList(); // cari data yang akan dihapus
                foreach (var x in detailList1)
                {
                    Barang_Outlet dataDetail = null; // inisialisasi object detail
                    if (x.barango_id > 0) // berarti sudah pernah disimpan ke db, lakukan update data
                    {
                        var query = (from q in tobeDeleteList where q.barango_id == x.barango_id select q);
                        if (query.Count() > 0) // jika data ditemukan, maka update data tersebut
                        {
                            dataDetail = query.First(); // ambil data yang akan diupdate
                            tobeDeleteList.Remove(dataDetail); // hapus dari list yang akan dihapus
                        }
                    }
                    else // berarti data baru, buat object baru
                    {
                        dataDetail = new Barang_Outlet();
                        _db.Barang_Outlets.Add(dataDetail); // tambahkan ke db
                        dataDetail.barang= oldObj; // set link ke object utama
                    }
                    dataDetail.outlet_id = x.outlet_id; // set data detailnya
                    _auditService.SetStsrcFields(dataDetail); // set kolom stsrc, date_created, created_by, date_modified dan modified_by
                }
                foreach (var x in tobeDeleteList) // hapus data yang sudah tidak ada di temporary table
                {
                    _auditService.DeleteStsrc(x); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)
                }

                // Barang Diskon
                var temp2 = new TempTableHelper<List<Barang_Diskon>>(_db); // buat objek helper untuk menyimpan data sementara
                Guid temptableGuid2 = Guid.Parse(obj.temptable_diskon_id); // ambil id temporary table dari object yang diisi ke tipe data Guid
                var detailList2 = temp2.GetContentAsObject(temptableGuid2); // ambil data dari temporary table berdasarkan id

                /* start input data detail ke DB (Add, Edit, Delete) */
                var tobeDeleteList2 = (from q in _db.Barang_Diskons where q.stsrc == "A" && q.barang_id == obj.barang_id select q).ToList(); // cari data yang akan dihapus
                foreach (var x in detailList2)
                {
                    Barang_Diskon dataDetail = null; // inisialisasi object detail
                    if (x.barangd_id > 0) // berarti sudah pernah disimpan ke db, lakukan update data
                    {
                        var query = (from q in tobeDeleteList2 where q.barangd_id == x.barangd_id select q);
                        if (query.Count() > 0) // jika data ditemukan, maka update data tersebut
                        {
                            dataDetail = query.First(); // ambil data yang akan diupdate
                            tobeDeleteList2.Remove(dataDetail); // hapus dari list yang akan dihapus
                        }
                    }
                    else // berarti data baru, buat object baru
                    {
                        dataDetail = new Barang_Diskon();
                        _db.Barang_Diskons.Add(dataDetail); // tambahkan ke db
                        dataDetail.barang = oldObj; // set link ke object utama
                    }
                    dataDetail.barangd_disc = x.barangd_disc; // set data detailnya
                    dataDetail.barangd_qty = x.barangd_qty; // set data detailnya
                    if (dataDetail.barangd_disc > (dataDetail.barangd_qty * obj.barang_harga))
                    {
                        return BadRequest(new { message = $"Nilai Diskon tidak boleh lebih besar dari harga" });
                    }
                    _auditService.SetStsrcFields(dataDetail); // set kolom stsrc, date_created, created_by, date_modified dan modified_by
                }
                foreach (var x in tobeDeleteList2) // hapus data yang sudah tidak ada di temporary table
                {
                    _auditService.DeleteStsrc(x); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)
                }
                /* end input data detail ke DB (Add, Edit, Delete) */

                _db.SaveChanges(); // simpan perubahan ke database
                return Ok(ApiResponse<BarangMasterUpdateDto>.Ok(obj));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE (Untuk Tombol "Delete")
        [HttpDelete("delete")]
        [Authorize(Policy = "CanDeleteBarang")]
        public IActionResult Delete([FromForm] long key)
        {
            try
            {
                var obj = _db.Barang_Masters.Find(key); // cari data berdasarkan id yang diberikan
                _auditService.DeleteStsrc(obj); // fungsi untuk menghapus data dengan mengisi stsrc menjadi 'D' (deleted)

                // Hapus data outlet
                foreach (var outlet in (obj.Barang_Outlets.Where(x => x.stsrc == "A")))
                {
                    _auditService.DeleteStsrc(outlet);
                }
                // Hapus data diskon
                foreach (var diskon in (obj.Barang_Diskons.Where(x => x.stsrc == "A")))
                {
                    _auditService.DeleteStsrc(diskon);
                }
                _db.SaveChanges(); // simpan perubahan ke database
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
            var isKodeExist = _db.Barang_Masters.Any(x => x.barang_kode == kode && x.barang_id != id && x.stsrc == "A");
            return isKodeExist;
        }

        [HttpGet("summary/diskons")]
        public object GetDiskonWithBarangId(long headerId, DataSourceLoadOptions loadOptions)
        {
            var detailData = _db.Barang_Diskons
                                .Where(s => s.barang_id == headerId && s.stsrc == "A") // Filter berdasarkan ID dari master
                                .Select(s => new {
                                    s.barangd_id,
                                    s.barangd_qty,
                                    s.barangd_disc
                                });

            return DataSourceLoader.Load(detailData, loadOptions);
        }

        [HttpGet("summary/outlets")]
        public object GetOutletWithBarangId(long headerId, DataSourceLoadOptions loadOptions)
        {
            var detailData = _db.Barang_Outlets
                                .Where(s => s.barang_id == headerId && s.stsrc == "A") // Filter berdasarkan ID dari master
                                .Select(s => new {
                                    s.barango_id,
                                    s.outlet.outlet_kode,
                                    s.outlet.outlet_nama
                                });

            return DataSourceLoader.Load(detailData, loadOptions);
        }

        [HttpGet("ref/klasifikasi")]
        public object GetRefKlasifikasiTree(DataSourceLoadOptions loadOptions)
        {
            var query = from child in _db.Klasifikasi_Masters
                        join parent in _db.Klasifikasi_Masters on child.klas_parent_id equals parent.klas_id into joinedGroup
                        from subItem in joinedGroup.DefaultIfEmpty()
                        where child.stsrc == "A"
                        select new
                        {
                            child.klas_id,
                            display = child.klas_kode + " - " + child.klas_nama,
                            child.klas_parent_id,
                            klas_parent_display = subItem == null ? null : subItem.klas_kode + " - " + subItem.klas_nama
                        };
            return DataSourceLoader.Load(query.ToList(), loadOptions);
        }

        [HttpGet("detail/outlet")]
        public object GetGridOutlet(DataSourceLoadOptions loadOptions, string temptable_outlet_id)
        {
            Guid temptableGuid = Guid.Parse(temptable_outlet_id); // konversi string ke Guid

            var temp = new TempTableHelper<List<BarangOutletDTO>>(_db); // buat objek helper untuk menyimpan data sementara
            var details = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id
            return DataSourceLoader.Load(details, loadOptions);
        }

        [HttpPost("detail/init/outlet")]
        public IActionResult InitGridOutlet([FromForm] long barang_id)
        {
            try
            {
                var barangOutletDtoList = _db.Barang_Outlets
                .Join(_db.Outlet_Masters, b => b.outlet_id, k => k.outlet_id, (b, k) => new { b, k })
                .Where(c => c.b.stsrc == "A" && c.b.barang_id == barang_id)
                .Select(c => new BarangOutletDTO
                {
                    outlet_id = c.b.outlet_id,
                    barang_id = c.b.barang_id,
                    barango_id = c.b.barango_id,
                    outlet_kode = c.k.outlet_kode,
                    outlet_nama = c.k.outlet_nama
                }).ToList();
                var temp1 = new TempTableHelper<List<BarangOutletDTO>>(_db); // buat objek helper untuk menyimpan data sementara
                var tid1 = temp1.CreateContent(barangOutletDtoList); // buat temporary table dan simpan data ke dalamnya
                var result = new
                {
                    temptable_outlet_id = tid1.ToString()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
            return DataSourceLoader.Load(query.ToList(), loadOptions);
        }

        [HttpPost("detail/outlet")]
        public IActionResult AddDataOutlet([FromForm] string outlet_id, [FromForm] string temptable_outlet_id)
        {
            Guid temptableGuid = Guid.Parse(temptable_outlet_id); // konversi string ke Guid
            var temp = new TempTableHelper<List<BarangOutletDTO>>(_db); // buat objek helper untuk menyimpan data sementara
            var listDetail = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id
            bool isDuplicate = listDetail.Any(v => v.outlet_id.ToString() == outlet_id);
            if (isDuplicate)
            {
                return BadRequest(new { message = "Outlet tidak boleh duplikat!" });
            }
            var dataOutlet = _db.Outlet_Masters.FirstOrDefault(e => e.stsrc.Equals("A") && e.outlet_id.Equals(long.Parse(outlet_id)));
            if (dataOutlet == null)
            {
                return BadRequest(new { message = "Outlet tidak ditemukan!" });
            }

            var dataDetail = new BarangOutletDTO();
            long minId = listDetail.OrderBy(x => x.barango_id).Select(x => Convert.ToInt64(x.barango_id)).FirstOrDefault(); // ambil id terkecil
            if (minId > 0) { minId = 0; }

            dataDetail.barango_id = minId - 1; // id baru akan menjadi id terkecil - 1
            dataDetail.outlet_id = Convert.ToInt64(outlet_id);
            dataDetail.outlet_kode = dataOutlet.outlet_kode;
            dataDetail.outlet_nama = dataOutlet.outlet_nama;

            listDetail.Add(dataDetail); // tambahkan data ke list
            temp.UpdateContent(temptableGuid, listDetail); // simpan kembali list ke temporary table

            var resp = "Data outlet berhasil ditambahkan"; // pesan sukses
            return new JsonResult(resp); // mengembalikan response JSON dengan status sukses dan pesan
        }

        [HttpDelete("detail/outlet")]
        public IActionResult DeleteDataOutlet([FromForm] string key, [FromForm] string temptable_outlet_id)
        {
            long id = Convert.ToInt64(key); // konversi key ke long
            Guid temptableGuid = Guid.Parse(temptable_outlet_id.ToString()); // konversi string ke Guid
            var temp = new TempTableHelper<List<BarangOutletDTO>>(_db); // buat objek helper untuk menyimpan data sementara
            var listDetail = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id

            var dataDetail = listDetail.Where(x => x.barango_id == id); // cari data detail berdasarkan id
            if (dataDetail.Count() > 0) // jika data ditemukan
            {
                listDetail.Remove(dataDetail.First()); // hapus data detail dari list
            }
            temp.UpdateContent(temptableGuid, listDetail); // simpan kembali list detail ke temporary table
            var resp = "Data outlet berhasil dihapus"; // pesan sukses
            return new JsonResult(resp); // mengembalikan response JSON dengan status sukses dan pesan
        }

        [HttpGet("detail/diskon")]
        public object GetGridDiskon(DataSourceLoadOptions loadOptions, string temptable_diskon_id)
        {
            Guid temptableGuid = Guid.Parse(temptable_diskon_id); // konversi string ke Guid

            var temp = new TempTableHelper<List<Barang_Diskon>>(_db); // buat objek helper untuk menyimpan data sementara
            var details = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id
            return DataSourceLoader.Load(details, loadOptions);
        }

        [HttpPost("detail/init/diskon")]
        public IActionResult InitGridDiskon([FromForm] long barang_id)
        {
            try
            {
                // Temp table untuk Barang Diskon
                var barangDiskonList = (from q in _db.Barang_Diskons where q.stsrc == "A" && q.barang_id == barang_id select q).ToList(); // ambil data dari database
                barangDiskonList.ForEach(data =>
                {
                    _db.Entry(data).State = Microsoft.EntityFrameworkCore.EntityState.Detached; // menghapus relasi FK PK setiap datanya
                });
                var temp2 = new TempTableHelper<List<Barang_Diskon>>(_db); // buat objek helper untuk menyimpan data sementara
                var tid2 = temp2.CreateContent(barangDiskonList); // buat temporary table dan simpan data ke dalamnya
                var result = new
                {
                    temptable_diskon_id = tid2.ToString()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("detail/diskon")]
        public IActionResult AddDataDiskon([FromForm] decimal diskon, [FromForm] int qty, [FromForm] decimal harga, [FromForm] string temptable_diskon_id)
        {
            Guid temptableGuid = Guid.Parse(temptable_diskon_id); // konversi string ke Guid
            var temp = new TempTableHelper<List<Barang_Diskon>>(_db); // buat objek helper untuk menyimpan data sementara
            var listDetail = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id
            bool isDuplicate = listDetail.Any(v => v.barangd_qty == qty);
            if (diskon > (qty * harga))
            {
                return BadRequest(new { message = "Nilai diskon tidak boleh lebih besar dari harga!" });
            }
            if (isDuplicate)
            {
                return BadRequest(new { message = "Kuantitas tidak boleh duplikat!" });
            }

            var dataDetail = new Barang_Diskon();
            long minId = listDetail.OrderBy(x => x.barangd_id).Select(x => Convert.ToInt64(x.barangd_id)).FirstOrDefault(); // ambil id terkecil
            if (minId > 0) { minId = 0; }

            dataDetail.barangd_id = minId - 1; // id baru akan menjadi id terkecil - 1
            dataDetail.barangd_disc = diskon;
            dataDetail.barangd_qty = qty;

            listDetail.Add(dataDetail); // tambahkan data ke list
            temp.UpdateContent(temptableGuid, listDetail); // simpan kembali list ke temporary table

            var resp = "Data diskon berhasil ditambahkan"; // pesan sukses
            return new JsonResult(resp); // mengembalikan response JSON dengan status sukses dan pesan
        }

        [HttpDelete("detail/diskon")]
        public IActionResult DeleteDataDiskon([FromForm] string key, [FromForm] string temptable_diskon_id)
        {
            long id = Convert.ToInt64(key); // konversi key ke long
            Guid temptableGuid = Guid.Parse(temptable_diskon_id.ToString()); // konversi string ke Guid
            var temp = new TempTableHelper<List<Barang_Diskon>>(_db); // buat objek helper untuk menyimpan data sementara
            var listDetail = temp.GetContentAsObject(temptableGuid); // ambil data dari temporary table berdasarkan id

            var dataDetail = listDetail.Where(x => x.barangd_id == id); // cari data detail berdasarkan id
            if (dataDetail.Count() > 0) // jika data ditemukan
            {
                listDetail.Remove(dataDetail.First()); // hapus data detail dari list
            }
            temp.UpdateContent(temptableGuid, listDetail); // simpan kembali list detail ke temporary table
            var resp = "Data diskon berhasil dihapus"; // pesan sukses
            return new JsonResult(resp); // mengembalikan response JSON dengan status sukses dan pesan
        }
    }
}
