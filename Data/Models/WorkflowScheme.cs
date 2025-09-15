using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowScheme
{
    public string Code { get; set; } = null!;

    public string Scheme { get; set; } = null!;

    public bool CanBeInlined { get; set; }

    public string? InlinedSchemes { get; set; }
}
