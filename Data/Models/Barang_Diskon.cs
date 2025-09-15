using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Barang_Diskon
{
    public long barangd_id { get; set; }

    public long? barang_id { get; set; }

    public int? barangd_qty { get; set; }

    public decimal? barangd_disc { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual Barang_Master? barang { get; set; }
}
