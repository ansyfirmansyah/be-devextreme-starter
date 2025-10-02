using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Lead_Source_Master
{
    public int lead_source_id { get; set; }

    public string lead_source_name { get; set; } = null!;

    public virtual ICollection<Contact_Master> Contact_Masters { get; set; } = new List<Contact_Master>();
}
