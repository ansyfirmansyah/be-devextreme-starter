using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowProcessInstance
{
    public Guid Id { get; set; }

    public string? StateName { get; set; }

    public string ActivityName { get; set; } = null!;

    public Guid? SchemeId { get; set; }

    public string? PreviousState { get; set; }

    public string? PreviousStateForDirect { get; set; }

    public string? PreviousStateForReverse { get; set; }

    public string? PreviousActivity { get; set; }

    public string? PreviousActivityForDirect { get; set; }

    public string? PreviousActivityForReverse { get; set; }

    public Guid? ParentProcessId { get; set; }

    public Guid RootProcessId { get; set; }

    public bool IsDeterminingParametersChanged { get; set; }
}
