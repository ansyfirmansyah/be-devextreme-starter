using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowInbox
{
    public Guid Id { get; set; }

    public Guid ProcessId { get; set; }

    public string IdentityId { get; set; } = null!;
}
