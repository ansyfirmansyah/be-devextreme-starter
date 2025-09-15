using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_User_Role
{
    public long urole_id { get; set; }

    public string user_id { get; set; } = null!;

    public string role_id { get; set; } = null!;

    public long? post_id { get; set; }

    public string? stsrc { get; set; }

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual FW_Ref_Role role { get; set; } = null!;

    public virtual User_Master user { get; set; } = null!;
}
