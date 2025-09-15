using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class CategoryLog
{
    public int CategoryLogID { get; set; }

    public int CategoryID { get; set; }

    public int LogID { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Log Log { get; set; } = null!;
}
