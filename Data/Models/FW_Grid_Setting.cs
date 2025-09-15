using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Grid_Setting
{
    public long gset_id { get; set; }

    public string gset_user { get; set; } = null!;

    public string gset_grid { get; set; } = null!;

    public string? gset_setting { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }
}
