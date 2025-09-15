using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowProcessInstancePersistence
{
    public Guid Id { get; set; }

    public Guid ProcessId { get; set; }

    public string ParameterName { get; set; } = null!;

    public string Value { get; set; } = null!;
}
