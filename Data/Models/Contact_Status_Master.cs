using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Contact_Status_Master
{
    public int contact_status_id { get; set; }

    public string contact_status_name { get; set; } = null!;

    public virtual ICollection<Contact_Master> Contact_Masters { get; set; } = new List<Contact_Master>();
}
