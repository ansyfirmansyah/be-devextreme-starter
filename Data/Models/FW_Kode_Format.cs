using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Kode_Format
{
    public string kof_id { get; set; } = null!;

    public byte kof_counter_length { get; set; }

    public byte kof_increment { get; set; }

    public int kof_start { get; set; }

    public string kof_catatan { get; set; } = null!;

    public string kof_reset_tp { get; set; } = null!;

    public int kof_reset_interval { get; set; }

    public DateTime kof_reset_time { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<FW_Kode_Counter> FW_Kode_Counters { get; set; } = new List<FW_Kode_Counter>();

    public virtual ICollection<FW_Kode_Detail> FW_Kode_Details { get; set; } = new List<FW_Kode_Detail>();
}
