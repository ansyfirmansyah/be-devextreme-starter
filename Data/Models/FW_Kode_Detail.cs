using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Kode_Detail
{
    public long kod_id { get; set; }

    public string kof_id { get; set; } = null!;

    public string kod_tipe { get; set; } = null!;

    public int kod_urut { get; set; }

    public byte? kod_length { get; set; }

    public string? kod_catatan { get; set; }

    public string? kod_char { get; set; }

    public string? kod_param_kode { get; set; }

    public bool kod_param_as_counter { get; set; }

    public virtual FW_Kode_Format kof { get; set; } = null!;
}
