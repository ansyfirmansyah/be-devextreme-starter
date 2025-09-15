using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowProcessTransitionHistory
{
    public Guid Id { get; set; }

    public Guid ProcessId { get; set; }

    public string? ExecutorIdentityId { get; set; }

    public string? ActorIdentityId { get; set; }

    public string FromActivityName { get; set; } = null!;

    public string ToActivityName { get; set; } = null!;

    public string? ToStateName { get; set; }

    public DateTime TransitionTime { get; set; }

    public string TransitionClassifier { get; set; } = null!;

    public bool IsFinalised { get; set; }

    public string? FromStateName { get; set; }

    public string? TriggerName { get; set; }
}
