using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Employee
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid StructDivisionId { get; set; }

    public bool IsHead { get; set; }

    public virtual ICollection<DocumentTransitionHistory> DocumentTransitionHistories { get; set; } = new List<DocumentTransitionHistory>();

    public virtual StructDivision StructDivision { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
