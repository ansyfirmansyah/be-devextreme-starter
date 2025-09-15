using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class vw_UserPermission
{
    public string user_id { get; set; } = null!;

    public string role_id { get; set; } = null!;

    public string mod_kode { get; set; } = null!;
}
