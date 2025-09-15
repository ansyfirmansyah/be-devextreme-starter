using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class ReportDef
{
    public string rdef_kode { get; set; } = null!;

    public byte[] rdef_def { get; set; } = null!;

    public string? rdef_nama { get; set; }

    public virtual ICollection<FW_Notice_Template> FW_Notice_Templates { get; set; } = new List<FW_Notice_Template>();
}
