using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class vHead
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid HeadId { get; set; }

    public string HeadName { get; set; } = null!;
}
