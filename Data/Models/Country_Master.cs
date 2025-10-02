using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Country_Master
{
    public int country_id { get; set; }

    public string country_name { get; set; } = null!;

    public virtual ICollection<City_Master> City_Masters { get; set; } = new List<City_Master>();
}
