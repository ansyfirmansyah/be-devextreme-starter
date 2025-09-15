using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Help
{
    public long help_id { get; set; }

    public int help_urut { get; set; }

    public string help_nama { get; set; } = null!;

    public string? help_catatan { get; set; }

    public DateTime? help_last_update { get; set; }

    public string? help_last_update_by { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<FW_Attachment> FW_Attachments { get; set; } = new List<FW_Attachment>();
}
