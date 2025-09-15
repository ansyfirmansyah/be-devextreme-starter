using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowProcessInstanceStatus
{
    public Guid Id { get; set; }

    public byte Status { get; set; }

    public Guid Lock { get; set; }
}
