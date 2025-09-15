using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Holiday
{
    public long holiday_id { get; set; }

    public DateTime holiday_date { get; set; }

    public string holiday_keterangan { get; set; } = null!;

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }
}
