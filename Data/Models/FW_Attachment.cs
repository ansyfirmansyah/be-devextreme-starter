using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Attachment
{
    public long attach_id { get; set; }

    public Guid attach_obj_id { get; set; }

    public string attach_tipe { get; set; } = null!;

    public string? attach_kode { get; set; }

    public byte[]? attach_thumb { get; set; }

    public string attach_file_nama { get; set; } = null!;

    public int attach_file_size { get; set; }

    public string? attach_file_link { get; set; }

    public string? attach_file_pwd { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public long? help_id { get; set; }

    public virtual Help? help { get; set; }
}
