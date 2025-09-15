using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class LOV
{
    public long lov_id { get; set; }

    public string lov_scope { get; set; } = null!;

    public string? lov_kode { get; set; }

    public string lov_nama { get; set; } = null!;

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }
}
