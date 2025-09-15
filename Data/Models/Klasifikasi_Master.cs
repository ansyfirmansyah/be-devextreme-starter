using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Klasifikasi_Master
{
    public long klas_id { get; set; }

    /// <summary>
    /// menggambarkan hubungan wilayah penjualan.
    /// Contoh : city anak dari branch
    /// </summary>
    public string? klas_kode { get; set; }

    public string klas_nama { get; set; } = null!;

    public long? klas_parent_id { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }
}
