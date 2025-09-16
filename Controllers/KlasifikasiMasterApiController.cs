using be_devextreme_starter.Data;
using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace be_devextreme_starter.Areas.API.Controllers
{
    [ApiController]
    [Route("api/klasifikasi")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class KlasifikasiMasterApiController : Controller
    {
        #region
        private DataEntities _db;
        private IWebHostEnvironment _env;
        public KlasifikasiMasterApiController(DataEntities context, IWebHostEnvironment env)
        {
            this._db = context;
            this._env = env;
        }
        #endregion

        // GET: /api/klasifikasi/get
        [HttpGet("get")]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var query = from child in _db.Klasifikasi_Masters
                            join parent in _db.Klasifikasi_Masters on child.klas_parent_id equals parent.klas_id into joinedGroup
                            from subItem in joinedGroup.DefaultIfEmpty()
                            where child.stsrc == "A"
                            select new
                            {
                                child.klas_id,
                                child.klas_kode,
                                child.klas_nama,
                                child.klas_parent_id,
                                display = child.klas_kode + " - " + child.klas_nama,
                                klas_parent_display = subItem == null ? null : subItem.klas_kode + " - " + subItem.klas_nama
                            };
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
                var newData = new Klasifikasi_Master();
                JsonConvert.PopulateObject(values, newData);
                if (checkDuplicateKode(newData.klas_kode, newData.klas_id))
                {
                    return BadRequest(new { message = $"Kode '{newData.klas_kode}' sudah digunakan. Silakan gunakan kode lain." });
                }
                _db.SetStsrcFields(newData);

                _db.Klasifikasi_Masters.Add(newData);
                _db.SaveChanges();
                return Ok(ApiResponse<Klasifikasi_Master>.Created(newData));
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
                var oldObj = _db.Klasifikasi_Masters.Find(key); // cari terlebih dahulu data sesuai id yang diubah
                if (oldObj == null)
                    return NotFound();
                var dto = JsonConvert.DeserializeObject<KlasifikasiUpdateDto>(values);
                if (checkDuplicateKode(dto.klas_kode, dto.klas_id))
                {
                    return BadRequest(new { message = $"Kode '{dto.klas_kode}' sudah digunakan. Silakan gunakan kode lain." });
                }
                // fungsi untuk copy data dari object edit ke object di database, sebutkan kolom-kolom yang ingin diubah
                WSMapper.CopyFieldValues(dto, oldObj, "klas_id,klas_kode,klas_nama,klas_parent_id");
                _db.SetStsrcFields(oldObj); // fungsi untuk mengisi stsrc, date_created, created_by, date_modified dan modified_by

                _db.SaveChanges(); // simpan perubahan ke database
                return Ok(ApiResponse<KlasifikasiUpdateDto>.Ok(dto));
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
                // Buat daftar data yang perlu dihapus (termasuk child nya)
                var listIds = new List<long?>();
                listIds = getChild(listIds, key);
                listIds.Add(key);

                // Periksa apakah sudah ada relasi ke barang
                bool checkRelation = _db.Barang_Masters.Any(b => b.stsrc == "A" && listIds.Contains(b.klas_id));
                if (checkRelation)
                {
                    var existingObj = _db.Klasifikasi_Masters.Find(key);
                    return BadRequest(new
                    {
                        error = $"Klasifikasi '{existingObj.klas_kode} - {existingObj.klas_nama}' tidak dapat dihapus karena sudah memiliki relasi ke data barang"
                    });
                }
                // Hapus semua data termasuk child-nya
                foreach (var id in listIds)
                {
                    var obj = _db.Klasifikasi_Masters.Find(id);
                    _db.DeleteStsrc(obj);
                }
                _db.SaveChanges(); // simpan perubahan ke database
                return Ok(ApiResponse<object>.Ok(null, "data deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("parent/lookup")]
        public object GetRefParent(DataSourceLoadOptions loadOptions, long klas_id)
        {
            var invalidParentId = new List<long?>();
            invalidParentId = getChild(invalidParentId, klas_id);
            invalidParentId.Add(klas_id);

            var query = _db.Klasifikasi_Masters
                //.Where(x => x.stsrc == "A" && x.klas_parent_id == null && x.klas_id != klas_id)
                .Where(x => x.stsrc == "A" && !invalidParentId.Contains(x.klas_id))
                .OrderBy(x => x.klas_nama)
                .Select(x => new
                {
                    x.klas_id,
                    x.klas_nama,
                    display = x.klas_kode + " - " + x.klas_nama
                });
            return DataSourceLoader.Load(query, loadOptions);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private List<long?> getChild(List<long?> childs, long? parentId)
        {
            // ambil semua child sesuai id parent nya
            var childrenIds = _db.Klasifikasi_Masters
                               .Where(item => item.klas_parent_id == parentId)
                               .Select(item => item.klas_id)
                               .ToList();
            // looping semua child
            childrenIds.ForEach(id => {
                // masukkan ke list
                childs.Add(id);
                // periksa child dari child yang sudah ditemukan
                getChild(childs, id);
            });
            return childs;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool checkDuplicateKode(string kode, long id)
        {
            // Jika id = 0 (mode Create)
            // Jika id > 0 (mode Edit)
            var isKodeExist = _db.Klasifikasi_Masters.Any(x => x.klas_kode == kode && x.klas_id != id && x.stsrc == "A");
            return isKodeExist;
        }
    }
}
