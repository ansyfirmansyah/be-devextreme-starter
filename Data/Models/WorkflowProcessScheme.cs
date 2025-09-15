using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class WorkflowProcessScheme
{
    public Guid Id { get; set; }

    public string Scheme { get; set; } = null!;

    public string DefiningParameters { get; set; } = null!;

    public string DefiningParametersHash { get; set; } = null!;

    public string SchemeCode { get; set; } = null!;

    public bool IsObsolete { get; set; }

    public string? RootSchemeCode { get; set; }

    public Guid? RootSchemeId { get; set; }

    public string? AllowedActivities { get; set; }

    public string? StartingTransition { get; set; }
}
