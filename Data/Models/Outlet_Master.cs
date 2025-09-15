using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Outlet_Master
{
    public long outlet_id { get; set; }

    /// <summary>
    /// menggambarkan hubungan wilayah penjualan.
    /// Contoh : city anak dari branch
    /// </summary>
    public string? outlet_kode { get; set; }

    public string outlet_nama { get; set; } = null!;

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<Barang_Outlet> Barang_Outlets { get; set; } = new List<Barang_Outlet>();

    public virtual ICollection<Jual_Header> Jual_Headers { get; set; } = new List<Jual_Header>();

    public virtual ICollection<Sales_Master> Sales_Masters { get; set; } = new List<Sales_Master>();
}
