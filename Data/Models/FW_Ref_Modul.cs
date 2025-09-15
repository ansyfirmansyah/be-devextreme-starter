using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Ref_Modul
{
    public string mod_kode { get; set; } = null!;

    public string mod_catatan { get; set; } = null!;

    public string? parent_mod_kode { get; set; }

    public virtual ICollection<FW_Role_Right> FW_Role_Rights { get; set; } = new List<FW_Role_Right>();
}
