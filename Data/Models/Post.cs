using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Post
{
    public long post_id { get; set; }

    public string post_nama { get; set; } = null!;

    public string post_tipe { get; set; } = null!;

    public long? parent_id { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual ICollection<Post> Inverseparent { get; set; } = new List<Post>();

    public virtual ICollection<Post_Child> Post_Childchild_posts { get; set; } = new List<Post_Child>();

    public virtual ICollection<Post_Child> Post_Childposts { get; set; } = new List<Post_Child>();

    public virtual ICollection<User_Master> User_Masters { get; set; } = new List<User_Master>();

    public virtual Post? parent { get; set; }
}
