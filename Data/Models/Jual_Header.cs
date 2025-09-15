using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Jual_Header
{
    public long jualh_id { get; set; }

    public string jualh_kode { get; set; } = null!;

    public DateTime jualh_date { get; set; }

    public long sales_id { get; set; }

    public long outlet_id { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<Jual_Detail> Jual_Details { get; set; } = new List<Jual_Detail>();

    public virtual Outlet_Master outlet { get; set; } = null!;

    public virtual Sales_Master sales { get; set; } = null!;
}
