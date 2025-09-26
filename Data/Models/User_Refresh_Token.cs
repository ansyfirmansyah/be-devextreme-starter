using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class User_Refresh_Token
{
    public long refresh_token_id { get; set; }

    public string user_id { get; set; } = null!;

    public string access_token { get; set; } = null!;

    public string refresh_token { get; set; } = null!;

    public string stsrc { get; set; } = null!;

    public DateTime date_expires { get; set; }

    public DateTime date_created { get; set; }

    public DateTime? date_updated { get; set; }

    public string? device_info { get; set; }

    public string? ip_address { get; set; }

    public string? created_by { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual User_Master user { get; set; } = null!;
}
