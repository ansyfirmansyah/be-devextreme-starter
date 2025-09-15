using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Temp_Table
{
    public Guid temp_id { get; set; }

    public string? temp_content2 { get; set; }

    public string temp_content { get; set; } = null!;

    public string user_id { get; set; } = null!;

    public DateTime? date_created { get; set; }

    public DateTime? date_modified { get; set; }
}
