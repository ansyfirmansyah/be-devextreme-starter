using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Category_DiffDate
{
    public long cd_id { get; set; }

    public string? cd_text { get; set; }

    public int? cd_range_first { get; set; }

    public int? cd_range_last { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }
}
