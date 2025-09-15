using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Sales_Master
{
    public long sales_id { get; set; }

    /// <summary>
    /// menggambarkan hubungan wilayah penjualan.
    /// Contoh : city anak dari branch
    /// </summary>
    public string? sales_kode { get; set; }

    public string sales_nama { get; set; } = null!;

    public long outlet_id { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<Jual_Header> Jual_Headers { get; set; } = new List<Jual_Header>();

    public virtual Outlet_Master outlet { get; set; } = null!;
}
