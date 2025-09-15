using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_LOG_Error
{
    public long err_id { get; set; }

    public string? err_ip_address { get; set; }

    public string? err_user_id { get; set; }

    public string? err_message { get; set; }

    public string? err_description { get; set; }

    public DateTime? err_date { get; set; }
}
