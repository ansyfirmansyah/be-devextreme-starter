using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Category
{
    public int CategoryID { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<CategoryLog> CategoryLogs { get; set; } = new List<CategoryLog>();
}
