using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowProcessTimer
{
    public Guid Id { get; set; }

    public Guid ProcessId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime NextExecutionDateTime { get; set; }

    public bool Ignore { get; set; }
}
