using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class DocumentTransitionHistory
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public Guid? EmployeeId { get; set; }

    public string AllowedToEmployeeNames { get; set; } = null!;

    public DateTime? TransitionTime { get; set; }

    public long Order { get; set; }

    public DateTime? TransitionTimeForSort { get; set; }

    public string InitialState { get; set; } = null!;

    public string DestinationState { get; set; } = null!;

    public string Command { get; set; } = null!;

    public virtual Employee? Employee { get; set; }
}
