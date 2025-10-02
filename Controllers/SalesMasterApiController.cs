using be_devextreme_starter.Data;
using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using be_devextreme_starter.Services;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace be_devextreme_starter.Controllers
{
    [ApiController]
    [Route("api/sales")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    [Tags("Sales")]
    public class SalesMasterApiController : Controller
    {
        private readonly DataEntities _db;
        private readonly IWebHostEnvironment _env;
        private readonly IValidator<SalesUpdateDto> _salesValidator;
        private readonly IAuditService _auditService;
        public SalesMasterApiController(DataEntities context, IWebHostEnvironment env, IValidator<SalesUpdateDto> salesValidator, IAuditService auditService)
        {
            this._db = context;
            this._env = env;
            this._salesValidator = salesValidator;
            this._auditService = auditService;
        }

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
        [Authorize(Policy = "CanCreateSales")]
        public async Task<IActionResult> Post([FromForm] string values)
        {
            var dto = new SalesUpdateDto();
            JsonConvert.PopulateObject(values, dto);

            // Jalankan validasi secara manual
            var validationResult = await _salesValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.BadRequest(validationResult.ToString(" ")));
            }

            var newData = new Sales_Master();
            WSMapper.CopyFieldValues(dto, newData, "sales_kode,sales_nama,outlet_id");
            _auditService.SetStsrcFields(newData);

            _db.Sales_Masters.Add(newData);
            _db.SaveChanges();
            return Ok(ApiResponse<Sales_Master>.Created(newData));
        }

        // UPDATE (Untuk Tombol "Edit")
        [HttpPut("put")]
        [Authorize(Policy = "CanEditSales")]
        public async Task<IActionResult> Put([FromForm] long key, [FromForm] string values)
        {
            try
            {
                var oldObj = _db.Sales_Masters.Find(key); // cari terlebih dahulu data sesuai id yang diubah
                if (oldObj == null)
                    return NotFound();
                var dto = new SalesUpdateDto();
                JsonConvert.PopulateObject(values, dto);

                // Jalankan validasi secara manual
                var validationResult = await _salesValidator.ValidateAsync(dto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.BadRequest(validationResult.ToString(" ")));
                }
                // fungsi untuk copy data dari object edit ke object di database, sebutkan kolom-kolom yang ingin diubah
                WSMapper.CopyFieldValues(dto, oldObj, "sales_id,sales_kode,sales_nama,outlet_id");
                _auditService.SetStsrcFields(oldObj); // fungsi untuk mengisi stsrc, date_created, created_by, date_modified dan modified_by

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
        [Authorize(Policy = "CanDeleteSales")]
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

                _auditService.DeleteStsrc(existingData);
                _db.SaveChanges();
                return Ok(ApiResponse<object>.Ok(null, "data deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("download-template")]
        [AllowAnonymous]
        public IActionResult DownloadTemplate()
        {
            // Mengatur nama file yang akan diunduh
            var fileName = "Template_Upload_Sales.xlsx";
            // Menggunakan MemoryStream agar tidak perlu menyimpan file fisik di server
            using (var stream = new MemoryStream())
            {
                // Membuat package Excel baru dengan EPPlus
                using (var package = new ExcelPackage(stream))
                {
                    // Menambahkan worksheet (lembar kerja) baru
                    var worksheet = package.Workbook.Worksheets.Add("Data Sales");

                    // --- Membuat Header Tabel ---
                    worksheet.Cells["A1"].Value = "Kode Sales";
                    worksheet.Cells["B1"].Value = "Nama Sales";
                    worksheet.Cells["C1"].Value = "Kode Outlet";

                    // --- Styling Header (karena sebagai judul) ---
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.Font.Name = "Calibri";
                        range.Style.Font.Size = 11;
                    }

                    // --- Menambahkan Contoh Data (Opsional, tapi sangat membantu pengguna) ---
                    worksheet.Cells["A2"].Value = "S01";
                    worksheet.Cells["B2"].Value = "Contoh Nama Sales";
                    worksheet.Cells["C2"].Value = "OK01"; // Pastikan kode outlet contoh ini ada di database
                    // --- Styling Detail agar sama dengan Header
                    using (var range = worksheet.Cells["A2:C2"])
                    {
                        range.Style.Font.Name = "Calibri";
                        range.Style.Font.Size = 11;
                    }

                    // Mengatur lebar kolom agar pas
                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();

                    // Simpan perubahan ke package
                    package.Save();
                }

                // Kembali ke awal stream sebelum dibaca untuk diunduh
                stream.Position = 0;

                // Mengembalikan file ke browser
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [HttpPost("preview-upload")]
        [Authorize(Policy = "CanUploadSales")]
        public async Task<IActionResult> PreviewUpload([FromForm] FileUploadRequest request)
        {
            // Mendapatkan file dari request body
            var file = request.File;
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "File tidak boleh kosong." });
            }

            // Menggunakan DTO sebagai containernya
            var previewList = new List<SalesUploadPreviewDto>();
            var kodes = new List<String>(); // untuk validasi duplikat kode di file yang sama
            int row = 1;

            // Menggunakan MemoryStream agar tidak perlu menyimpan file fisik di server
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                // Membuat package Excel baru dengan EPPlus
                using (var package = new ExcelPackage(stream))
                {
                    // Hanya membaca sheet pertama
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    if (worksheet.Dimension == null)
                    {
                        return BadRequest(new { message = "File tidak boleh kosong." });
                    }
                    // Menghitung isi baris
                    var rowCount = worksheet.Dimension.Rows;

                    // Validasi judul kolom
                    if (worksheet.Cells["A1"].Value?.ToString().Trim().ToLower() != "kode sales"
                        || worksheet.Cells["B1"].Value?.ToString().Trim().ToLower() != "nama sales"
                        || worksheet.Cells["C1"].Value?.ToString().Trim().ToLower() != "kode outlet"
                        )
                    {
                        return BadRequest(new { message = "Judul kolom tidak sesuai." });
                    }

                    for (row = 2; row <= rowCount; row++) // Asumsi baris pertama adalah header maka di-skip
                    {
                        var previewDto = new SalesUploadPreviewDto { 
                            RowNumber = row - 1, // row number akan digunakan sebagai nomor, oleh karena itu dikurang 1 dari index
                            sales_kode = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                            sales_nama = worksheet.Cells[row, 2].Value?.ToString().Trim(),
                            outlet_kode = worksheet.Cells[row, 3].Value?.ToString().Trim(),
                            IsValid = true
                        };
                        List<String> errorMessages = new List<String>();

                        // --- Validasi ---
                        if (string.IsNullOrEmpty(previewDto.sales_kode))
                        {
                            previewDto.IsValid = false;
                            errorMessages.Add("Kode Sales harus diisi.");
                        } else
                        {
                            // cek kode duplikat
                            if (kodes.Any(a => a == previewDto.sales_kode))
                            {
                                previewDto.IsValid = false;
                                errorMessages.Add($"Kode sales '{previewDto.sales_kode}' sudah ada di file yang diupload.");
                            }
                            // add existing kode untuk validasi
                            kodes.Add(previewDto.sales_kode);
                            if (_db.Sales_Masters.Any(s => s.sales_kode == previewDto.sales_kode && s.stsrc == "A"))
                            {
                                previewDto.IsValid = false;
                                errorMessages.Add($"Kode sales '{previewDto.sales_kode}' sudah ada di database.");
                            }
                        }

                        if (string.IsNullOrEmpty(previewDto.sales_nama))
                        {
                            previewDto.IsValid = false;
                            errorMessages.Add("Nama Sales harus diisi.");
                        }
                        if (string.IsNullOrEmpty(previewDto.outlet_kode))
                        {
                            previewDto.IsValid = false;
                            errorMessages.Add("Kode Outlet harus diisi.");
                        } else if (!_db.Outlet_Masters.Any(s => s.outlet_kode == previewDto.outlet_kode && s.stsrc == "A"))
                        {
                            previewDto.IsValid = false;
                            errorMessages.Add($"Kode outlet '{previewDto.outlet_kode}' tidak ditemukan.");
                        }       

                        // Kombinasikan error message
                        if (!previewDto.IsValid)
                        {
                            foreach (var item in errorMessages)
                            {
                                previewDto.ValidationMessage += (item + " ");
                            }
                        }
                            
                        previewList.Add(previewDto);
                    }
                }
            }

            return Ok(ApiResponse<List<SalesUploadPreviewDto>>.Ok(previewList));
        }

        [HttpPost("commit-upload")]
        [Authorize(Policy = "CanUploadSales")]
        public async Task<IActionResult> CommitUpload([FromBody] List<SalesUploadPreviewDto> validSales)
        {
            // Periksa isi request body
            if (validSales == null || !validSales.Any())
            {
                return BadRequest(new { message = "Tidak ada data valid untuk diimpor." });
            }
            var salesToCreate = new List<Sales_Master>();

            // Insert ke database menggunakan transaction agar menjadi atomic (semua berhasil atau semua gagal total)
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    // Looping isi request body
                    foreach (var saleDto in validSales)
                    {
                        // Kita perlu mengambil lagi outlet_id karena DTO hanya punya outlet_kode
                        var outlet = _db.Outlet_Masters.FirstOrDefault(o => o.outlet_kode == saleDto.outlet_kode && o.stsrc == "A");
                        if (outlet == null)
                        {
                            // Jika outlet tidak ditemukan, lemparkan error yang lebih jelas
                            return BadRequest(new { message = $"Gagal menyimpan: Outlet dengan kode '{saleDto.outlet_kode}' tidak ditemukan di database." });
                        }
                        // Init data sesuai kebutuhan database
                        var newSale = new Sales_Master
                        {
                            sales_kode = saleDto.sales_kode,
                            sales_nama = saleDto.sales_nama,
                            outlet_id = outlet.outlet_id
                        };
                        _auditService.SetStsrcFields(newSale);
                        salesToCreate.Add(newSale);
                    }

                    await _db.Sales_Masters.AddRangeAsync(salesToCreate);
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

            return Ok(new { message = $"{salesToCreate.Count} data berhasil diimpor." });
        }
    }
}
