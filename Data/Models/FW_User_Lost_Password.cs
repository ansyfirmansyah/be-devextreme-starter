using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_User_Lost_Password
{
    public long ulp_id { get; set; }

    public DateTime ulp_date { get; set; }

    public string user_id { get; set; } = null!;

    public DateTime ulp_expire_date { get; set; }

    public string ulp_email { get; set; } = null!;

    public string ulp_code { get; set; } = null!;

    public string ulp_status { get; set; } = null!;

    public DateTime? ulp_reset_date { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual User_Master user { get; set; } = null!;
}
