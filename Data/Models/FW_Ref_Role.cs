using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Ref_Role
{
    public string role_id { get; set; } = null!;

    public string role_catatan { get; set; } = null!;

    public int? urut { get; set; }

    public string? role_scope { get; set; }

    public string? stsrc { get; set; }

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<FW_Role_Right> FW_Role_Rights { get; set; } = new List<FW_Role_Right>();

    public virtual ICollection<FW_User_Role> FW_User_Roles { get; set; } = new List<FW_User_Role>();
}
