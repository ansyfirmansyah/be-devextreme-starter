using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Barang_Master
{
    public long barang_id { get; set; }

    /// <summary>
    /// menggambarkan hubungan wilayah penjualan.
    /// Contoh : city anak dari branch
    /// </summary>
    public string? barang_kode { get; set; }

    public string barang_nama { get; set; } = null!;

    public long klas_id { get; set; }

    public decimal barang_harga { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<Barang_Diskon> Barang_Diskons { get; set; } = new List<Barang_Diskon>();

    public virtual ICollection<Barang_Outlet> Barang_Outlets { get; set; } = new List<Barang_Outlet>();

    public virtual ICollection<Jual_Detail> Jual_Details { get; set; } = new List<Jual_Detail>();
}
