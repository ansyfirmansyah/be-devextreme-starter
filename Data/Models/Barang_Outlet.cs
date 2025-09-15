using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Barang_Outlet
{
    public long barango_id { get; set; }

    public long barang_id { get; set; }

    public long outlet_id { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual Barang_Master barang { get; set; } = null!;

    public virtual Outlet_Master outlet { get; set; } = null!;
}
