using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Ref_Setting
{
    public string set_name { get; set; } = null!;

    public string set_type { get; set; } = null!;

    public string set_value { get; set; } = null!;

    public string? set_catatan { get; set; }

    public string? set_group { get; set; }

    public bool set_foruser { get; set; }
}
