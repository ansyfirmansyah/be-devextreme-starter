using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Workday
{
    public long workday_id { get; set; }

    public short workday_year { get; set; }

    public string workday_dayName { get; set; } = null!;

    public DateTime? workday_start { get; set; }

    public DateTime? workday_end { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public byte? workday_day { get; set; }

    public string calendar_type { get; set; } = null!;
}
