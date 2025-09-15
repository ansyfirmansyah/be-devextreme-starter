using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Jual_Detail
{
    public long juald_id { get; set; }

    public long jualh_id { get; set; }

    public long barang_id { get; set; }

    public int juald_qty { get; set; }

    public decimal juald_harga { get; set; }

    public decimal juald_disk { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual Barang_Master barang { get; set; } = null!;

    public virtual Jual_Header jualh { get; set; } = null!;
}
