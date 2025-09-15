using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Kode_Counter
{
    public long kdcn_id { get; set; }

    public string kof_id { get; set; } = null!;

    public string kdcn_prefix { get; set; } = null!;

    public int kdcn_counter { get; set; }

    public string? kdcn_last { get; set; }

    public DateTime kdcn_last_update { get; set; }

    public DateTime kdcn_last_reset { get; set; }

    public virtual FW_Kode_Format kof { get; set; } = null!;
}
