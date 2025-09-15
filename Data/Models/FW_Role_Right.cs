using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Role_Right
{
    public long right_id { get; set; }

    public string role_id { get; set; } = null!;

    public string mod_kode { get; set; } = null!;

    public string? stsrc { get; set; }

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual FW_Ref_Modul mod_kodeNavigation { get; set; } = null!;

    public virtual FW_Ref_Role role { get; set; } = null!;
}
