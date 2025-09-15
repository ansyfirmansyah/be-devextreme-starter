using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class FW_Notice_Queue
{
    public long notq_id { get; set; }

    public string nott_id { get; set; } = null!;

    public DateTime nott_date { get; set; }

    public string? notq_title { get; set; }

    public DateTime? nott_sent_date { get; set; }

    public string? notq_user { get; set; }

    public string? notq_content { get; set; }

    public byte notq_status { get; set; }

    public long? emailq_id { get; set; }

    public string? notq_email { get; set; }

    public string stsrc { get; set; } = null!;

    public string? created_by { get; set; }

    public DateTime? date_created { get; set; }

    public string? modified_by { get; set; }

    public DateTime? date_modified { get; set; }

    public virtual FW_Email_Queue? emailq { get; set; }

    public virtual User_Master? notq_userNavigation { get; set; }

    public virtual FW_Notice_Template nott { get; set; } = null!;
}
