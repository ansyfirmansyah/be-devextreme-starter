using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}
