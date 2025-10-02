using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class City_Master
{
    public int city_id { get; set; }

    public string city_name { get; set; } = null!;

    public int country_id { get; set; }

    public virtual ICollection<Contact_Master> Contact_Masters { get; set; } = new List<Contact_Master>();

    public virtual Country_Master country { get; set; } = null!;
}
