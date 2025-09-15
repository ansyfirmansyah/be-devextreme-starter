using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Log
{
    public int LogID { get; set; }

    public int? EventID { get; set; }

    public int Priority { get; set; }

    public string Severity { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string MachineName { get; set; } = null!;

    public string AppDomainName { get; set; } = null!;

    public string ProcessID { get; set; } = null!;

    public string ProcessName { get; set; } = null!;

    public string? ThreadName { get; set; }

    public string? Win32ThreadId { get; set; }

    public string? Message { get; set; }

    public string? FormattedMessage { get; set; }

    public virtual ICollection<CategoryLog> CategoryLogs { get; set; } = new List<CategoryLog>();
}
