using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace be_devextreme_starter.Data.Models
{
    [ModelMetadataType(typeof(Barang_MasterMeta))]
    public partial class Barang_Master
    {

    }
    public class Barang_MasterMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Kode Barang")]
        [StringLength(10, ErrorMessage = "Panjang [{0}] Maksimal 10 Karakter")]
        public string barang_kode { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Nama Barang")]
        [StringLength(100, ErrorMessage = "Panjang [{0}] Maksimal 100 Karakter")]
        public string barang_nama { get; set; }

        [Required(ErrorMessage = "[{0}] harus dipilih")]
        [Display(Name = "Klasifikasi")]
        [Range(1, long.MaxValue, ErrorMessage = "[{0}] harus dipilih")]
        public long klas_id { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Harga")]
        [Range(0, double.MaxValue, ErrorMessage = "[{0}] harus diisi > 0")]
        public decimal barang_harga { get; set; }
    }
}
