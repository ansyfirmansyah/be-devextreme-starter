using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Post_Child
{
    public long post_id { get; set; }

    public long child_post_id { get; set; }

    public long pc_id { get; set; }

    public virtual Post child_post { get; set; } = null!;

    public virtual Post post { get; set; } = null!;
}
