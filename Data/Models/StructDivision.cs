using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class StructDivision
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<StructDivision> InverseParent { get; set; } = new List<StructDivision>();

    public virtual StructDivision? Parent { get; set; }
}
