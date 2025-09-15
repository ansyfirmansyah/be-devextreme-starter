using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Calendar
{
    public string calendar_id { get; set; } = null!;

    public DateTime calendar_date { get; set; }

    public string calendar_dayName { get; set; } = null!;

    public string calendar_monthName { get; set; } = null!;

    public short calendar_year { get; set; }

    public byte calendar_day { get; set; }

    public byte calendar_month { get; set; }

    public byte calendar_quarter { get; set; }

    public bool calendar_holiday { get; set; }

    public string? calendar_holiday_keterangan { get; set; }

    public string calendar_type { get; set; } = null!;
}
